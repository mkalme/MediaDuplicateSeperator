using System;
using System.Diagnostics;

namespace MediaDuplicateSeperator {
    static class ProcessUtilities {
        public static string ExecuteProcess(string exePath, string arguments, int milliseconds = 0) {
            using (Process p = new Process()) {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = arguments;
                p.Start();

                if (milliseconds == 0) p.WaitForExit();
                else p.WaitForExit(milliseconds);

                return p.StandardOutput.ReadToEnd();
            }
        }
        public static int ExecuteProcessWithExitCode(string exePath, string arguments, int milliseconds = 0) {
            using (Process p = new Process()) {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = arguments;
                p.Start();

                p.BeginOutputReadLine();
                string tmpErrorOut = p.StandardError.ReadToEnd();

                if (milliseconds == 0) p.WaitForExit();
                else p.WaitForExit(milliseconds);

                return p.ExitCode;
            }
        }
    }
}
