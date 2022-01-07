using BobcatExamples.WPF.GameBasis;
using ProjBobcat.Class.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace BobcatExamples.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            /*
             Due to the limitation of the default number of connections in .NET, 
            you need to manually override the default number of connections to 
            ensure that some methods in are executed normally. You can add the 
            following code in App.xaml.cs or the entry point of the program to 
            complete the modification (The maximum value should not exceed 1024).
             */
            ServicePointManager.DefaultConnectionLimit = 512;

            /*
             Initialize the basic helpers of Projbobcat.
             */
            ServiceHelper.Init();
            HttpClientHelper.Init();

            // Initialize the launcher core.
            Core.CoreInit();
        }
    }
}
