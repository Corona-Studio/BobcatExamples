using ProjBobcat.Class.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobcatExamples.WPF.JavaWorkaround
{
    public static class JavaDetection
    {
        public static IEnumerable<string> DetectJava()
        {
            // Returns a list of all java installations found in registry.
            return SystemInfoHelper.FindJava();
        }
    }
}
