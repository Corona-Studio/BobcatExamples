using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BobcatExamples.WPF.GameBasis;
using BobcatExamples.WPF.JavaWorkaround;
using ProjBobcat.Class.Model;
using ProjBobcat.Class.Model.LauncherProfile;
using ProjBobcat.DefaultComponent;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.DefaultComponent.ResourceInfoResolver;
using ProjBobcat.Event;
using ProjBobcat.Interface;

namespace BobcatExamples.WPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<VersionInfo> gameList;

        /*
         * You should declare the following variables 
         * in another file in production environment.
         * 
         * In this demo, I declare here for convenience.
         */
        private IEnumerable<string> javaList;

        public MainWindow()
        {
            InitializeComponent();

            // Necessary UI preparation. (does not really matter)
            RefreshJavaList();

            RefreshGameList();
        }

        /// <summary>
        ///     Refreshes the list of Java(s) installed.
        /// </summary>
        private void RefreshJavaList()
        {
            javaList = JavaDetection.DetectJava();
            JavaListView.ItemsSource = javaList;
        }

        /// <summary>
        ///     Refreshes the list of Game(s) in .minecraft/ directory.
        /// </summary>
        private void RefreshGameList()
        {
            gameList = Core.core.VersionLocator.GetAllGames().ToList();
            GameListView.ItemsSource = gameList;
        }

        private async Task DownloadResourcesAsync(VersionInfo versionInfo)
        {
            var versions = await Core.GetVersionManifestTaskAsync();
            var rc = new DefaultResourceCompleter
            {
                CheckFile = true,
                DownloadParts = 8,
                ResourceInfoResolvers = new List<IResourceInfoResolver>
                {
                    new VersionInfoResolver
                    {
                        BasePath = Core.core.RootPath,
                        VersionInfo = versionInfo,
                        CheckLocalFiles = true
                    },
                    new AssetInfoResolver
                    {
                        BasePath = Core.core.RootPath,
                        VersionInfo = versionInfo,
                        CheckLocalFiles = true,
                        Versions = versions?.Versions,
                        MaxDegreeOfParallelism = 8
                    },
                    new LibraryInfoResolver
                    {
                        BasePath = Core.core.RootPath,
                        VersionInfo = versionInfo,
                        CheckLocalFiles = true,
                        MaxDegreeOfParallelism = 8
                    }
                },
                TotalRetry = 2
            };

            rc.DownloadFileCompletedEvent += (sender, args) =>
            {
                if (sender is not DownloadFile file) return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    /*
                    var isSuccess = args.Success == null
                        ? string.Empty
                        : $"[{(args.Success.Value ? "Succeeded" : "Failed")}]";
                    var retry = file.RetryCount == 0
                        ? string.Empty
                        : $"<Retry - {file.RetryCount}>";

                    var fileName = file.FileType switch
                    {
                        ResourceType.Asset => file.FileName.AsSpan()[..10],
                        ResourceType.LibraryOrNative => file.FileName,
                        _ => file.FileName
                    };
                    var pD =
                        $"<{file.FileType} Completed>{retry}{isSuccess} {fileName.ToString()} ({rc.TotalDownloaded} / {rc.NeedToDownload}) [{args.AverageSpeed:F} Kb / s]";

                    GameLaunchLogs.AppendText($"[Resource Completer] - {pD}\n");
                    */

                    RcProgress.Value = (double) rc.TotalDownloaded / rc.NeedToDownload * 100;
                });
            };

            await rc.CheckAndDownloadTaskAsync();
        }

        //TODO: make UI events as a region.
        private void RefJavaBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshJavaList();
        }

        private void RefGameListBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshGameList();
        }

        private async void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (JavaListView.SelectedIndex == -1 || GameListView.SelectedIndex == -1)
            {
                MessageBox.Show("Select java and a game first.");
                return;
            }

            if (GameListView.SelectedItem is not VersionInfo versionInfo) return;

            var launchSettings = new LaunchSettings
            {
                FallBackGameArguments = new GameArguments
                {
                    GcType = GcType.G1Gc,
                    JavaExecutable = JavaListView.SelectedItem.ToString(),
                    Resolution = new ResolutionModel
                    {
                        Height = 600,
                        Width = 800
                    },
                    MinMemory = 512,
                    MaxMemory = 1024
                },
                Version = versionInfo.Id,
                GameName = versionInfo.Name,
                VersionInsulation = false,
                GameResourcePath = Core.core.RootPath,
                GamePath = Core.core.RootPath,
                VersionLocator = Core.core.VersionLocator,
                Authenticator = new OfflineAuthenticator //离线认证
                {
                    Username = OfflUN.Text, //离线用户名
                    LauncherAccountParser = Core.core.VersionLocator.LauncherAccountParser
                }
            };

            await DownloadResourcesAsync(versionInfo);

            Core.core.LaunchLogEventDelegate += Core_LaunchLogEventDelegate;
            Core.core.GameLogEventDelegate += Core_GameLogEventDelegate;
            Core.core.GameExitEventDelegate += Core_GameExitEventDelegate;

            var result = await Core.core.LaunchTaskAsync(launchSettings);

            GameLaunchLogs.AppendText(result.Error?.Exception.ToString());
        }

        private void Core_GameExitEventDelegate(object sender, GameExitEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { GameLaunchLogs.AppendText("Game exited."); });
        }

        private void Core_GameLogEventDelegate(object sender, GameLogEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { GameLaunchLogs.AppendText($"[Game Log] - {e.Content}\n"); });
        }

        private void Core_LaunchLogEventDelegate(object sender, LaunchLogEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { GameLaunchLogs.AppendText($"[Bobcat Log] - {e.Item}\n"); });
        }
    }
}