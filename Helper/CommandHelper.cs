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
    }
}
