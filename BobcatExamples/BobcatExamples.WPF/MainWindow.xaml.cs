using BobcatExamples.WPF.GameBasis;
using ProjBobcat.Class.Model;
using ProjBobcat.Class.Model.LauncherProfile;
using ProjBobcat.DefaultComponent.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BobcatExamples.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
         * You should declare the following variables 
         * in another file in production environment.
         * 
         * In this demo, I declare here for convenience.
         */
        IEnumerable<string> javaList;
        List<VersionInfo> gameList;

        public MainWindow()
        {
            InitializeComponent();

            // Necessary UI preparation. (does not really matter)
            RefreshJavaList();

            RefreshGameList();
        }

        /// <summary>
        /// Refreshes the list of Java(s) installed.
        /// </summary>
        private void RefreshJavaList()
        {
            javaList = JavaWorkaround.JavaDetection.DetectJava();
            JavaListView.ItemsSource = javaList;
        }

        /// <summary>
        /// Refreshes the list of Game(s) in .minecraft/ directory.
        /// </summary>
        private void RefreshGameList()
        {
            gameList = Core.core.VersionLocator.GetAllGames().ToList();
            GameListView.ItemsSource = gameList;
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
                Version = (GameListView.SelectedItem as VersionInfo).Id,
                GameName = (GameListView.SelectedItem as VersionInfo).Name,
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
            Core.core.LaunchLogEventDelegate += Core_LaunchLogEventDelegate;
            Core.core.GameLogEventDelegate += Core_GameLogEventDelegate;
            Core.core.GameExitEventDelegate += Core_GameExitEventDelegate;
            var result = await Core.core.LaunchTaskAsync(launchSettings);
            GameLaunchLogs.AppendText(result.Error?.Exception.ToString());
        }

        private void Core_GameExitEventDelegate(object sender, ProjBobcat.Event.GameExitEventArgs e)
        {
            GameLaunchLogs.AppendText("Game exited.");
        }

        private void Core_GameLogEventDelegate(object sender, ProjBobcat.Event.GameLogEventArgs e)
        {
            GameLaunchLogs.AppendText($"[Game Log] - {e.Content}\n");
        }

        private void Core_LaunchLogEventDelegate(object sender, ProjBobcat.Event.LaunchLogEventArgs e)
        {
            GameLaunchLogs.AppendText($"[Bobcat Log] - {e.Item}\n");
        }
    }
}
