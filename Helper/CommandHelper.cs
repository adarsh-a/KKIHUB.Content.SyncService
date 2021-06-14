using System;
using System.Diagnostics;

namespace KKIHUB.Content.SyncService.Helper
{
    public static class CommandHelper
    {
        public static void ExcecuteScript(string filePath)
        {
            try
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy unrestricted -File \"{filePath}\"",
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }

            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        public static string ExcecuteScriptOutput(string filePath)
        {
            var finalOutputMessage = string.Empty;
            try
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy unrestricted -File \"{filePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                var process = Process.Start(startInfo);
                
                while (!process.StandardOutput.EndOfStream)
                {
                    finalOutputMessage = process.StandardOutput.ReadLine();
                }
            }

            catch (Exception err)
            {
                Debug.WriteLine(err);
                finalOutputMessage = err.ToString();
            }

            return finalOutputMessage;
        }
    }
}
