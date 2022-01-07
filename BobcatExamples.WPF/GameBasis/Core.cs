using ProjBobcat.DefaultComponent.Launch;
using ProjBobcat.DefaultComponent.Launch.GameCore;
using ProjBobcat.DefaultComponent.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobcatExamples.WPF.GameBasis
{
    public static class Core
    {
        public static DefaultGameCore core;
        public static string rootPath;
        public static Guid clientToken;
        public static void CoreInit()
        {
            rootPath = Environment.CurrentDirectory+"\\.minecraft";
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
    }
}
