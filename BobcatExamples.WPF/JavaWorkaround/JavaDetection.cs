using System.Collections.Generic;
using ProjBobcat.Class.Helper;

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