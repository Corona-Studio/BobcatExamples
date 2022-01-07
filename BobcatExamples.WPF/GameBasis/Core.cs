using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model.Mojang;
using ProjBobcat.DefaultComponent.Launch;
using ProjBobcat.DefaultComponent.Launch.GameCore;
using ProjBobcat.DefaultComponent.Logging;

namespace BobcatExamples.WPF.GameBasis
{
    public static class Core
    {
        public static DefaultGameCore core;
        public static string rootPath;
        public static Guid clientToken;

        public static void CoreInit()
        {
            rootPath = Environment.CurrentDirectory + "\\.minecraft";
            clientToken = Guid.NewGuid();
            core = new DefaultGameCore
            {
                ClientToken = clientToken, // Pick any GUID as you like, and it does not affect launching.
                RootPath = rootPath,
                VersionLocator = new DefaultVersionLocator(rootPath, clientToken)
                {
                    LauncherProfileParser = new DefaultLauncherProfileParser(rootPath, clientToken),
                    LauncherAccountParser = new DefaultLauncherAccountParser(rootPath, clientToken)
                },
                GameLogResolver = new DefaultGameLogResolver()
            };
        }

        public static async Task<VersionManifest?> GetVersionManifestTaskAsync()
        {
            const string vmUrl = "http://launchermeta.mojang.com/mc/game/version_manifest.json";
            var contentRes = await HttpHelper.Get(vmUrl);
            var content = await contentRes.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<VersionManifest>(content);

            return model;
        }
    }
}