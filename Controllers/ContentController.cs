using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KKIHUB.Content.SyncService.Helper;
using KKIHUB.Content.SyncService.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KKIHUB.Content.SyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : Controller
    {
        private IContentService ContentService { get; set; }
        public ContentController(IContentService contentService)
        {
            this.ContentService = contentService;
        }


        [HttpGet]
        [Route("SyncComplete")]
        public async Task<IActionResult> SyncContentAsync(int days, string sourceHub, string targetHub)
        {
            var content = await ContentService.FetchContentAsync(days, sourceHub, false, false);

            return Json(content);
        }


        [HttpGet]
        [Route("SyncRecursive")]
        public async Task<IActionResult> SyncContentRecursive(int days, string sourceHub, string targetHub)
        {
            var content = await ContentService.FetchContentAsync(days, sourceHub, true, false);

            return Json(content);
        }

        [HttpGet]
        [Route("SyncUpdated")]
        public async Task<IActionResult> SyncContentUpdated(int days, string sourceHub, string targetHub)
        {
            var content = await ContentService.FetchContentAsync(days, sourceHub, true, true);

            return Json(content);
        }

        [HttpGet]
        [Route("ContentByLibrary")]
        public async Task<IActionResult> ContentByLibrary(string sourceHub, string libraryId)
        {
            var content = await ContentService.FetchContentByLibrary(sourceHub, libraryId);

            return Json(content);
        }

        [HttpGet]
        [Route("PushContent")]
        public IActionResult PushContent(string filepaths)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(filepaths))
            {
                try
                {
                    //Dictionary<string, string> pushData = JsonConvert.DeserializeObject<Dictionary<string, string>>(pushParams);
                    var files = filepaths;
                    //var targetHub = pushData["targethub"];

                    if (!string.IsNullOrEmpty(files))
                    {
                        var filePath = files.Split("|").ToList();

                        var contentList = JsonCreator.ListContent("content");
                        if (contentList.Any() && filePath.Any())
                        {
                            var itemsToDelete = contentList.Except(filePath).ToList();
                            if (itemsToDelete.Any())
                            {
                                var flag = JsonCreator.Delete("content", itemsToDelete);
                            }
                            message = CommandHelper.ExcecuteScriptOutput(Path.Combine(Environment.CurrentDirectory, Constants.Constants.Path.WchtoolsPath));
                        }


                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Error pushing content at {ex.Message} ");
                    message = ex.Message;
                }
            }

            //var content = await ContentService.FetchContentByLibrary(sourceHub, libraryId);

            return Json(message);
        }


        private void ExecuteCommand()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, Constants.Constants.Path.ArtifactPath, "wchtools_non-prod.cmd");
            //System.Diagnostics.Process.Start(filePath);


            var process = new Process();
            //var startinfo = new ProcessStartInfo(filePath, "\"1st_arg\" \"2nd_arg\" \"3rd_arg\"");
            var startinfo = new ProcessStartInfo("cmd.exe", "/c " + filePath);
            startinfo.RedirectStandardOutput = true;
            startinfo.UseShellExecute = false;
            process.StartInfo = startinfo;
            process.OutputDataReceived += (sender, argsx) => Console.WriteLine(argsx.Data); // do whatever processing you need to do in this handler
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }


        static void ExecuteCommandInApp(string command, string workingDirectory)
        {
            int exitCode;

            var nodePath = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Microsoft\VisualStudio\NodeJs\win-x64\node.exe";
            string initCommand = "wchtools init --url https://content-eu-1.content-cms.com/api/37dd7bf6-5628-4aac-8464-f4894ddfb8c4 --user adarsh.bhautoo@hangarww.com --password Ad1108bh_hangarMU";

            var processInfo = new ProcessStartInfo(nodePath, "/c " + initCommand);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.WorkingDirectory = workingDirectory;
            processInfo.RedirectStandardOutput = true;

            var process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("output>>" + e.Data);
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("error>>" + e.Data);
            process.BeginErrorReadLine();

            process.WaitForExit();

            Console.WriteLine("ExitCode: {0}", process.ExitCode);
            process.Close();
        }



        private void SyncContent()
        {

            var filePath = Path.Combine(Environment.CurrentDirectory, Constants.Constants.Path.ArtifactPath);

            //string installWchtools = "call npx -i -g --production --no-optional wchtools-cli";
            // ExecuteCommandInApp(installWchtools, filePath);
            //string initCommand = "npx wchtools init --url https://content-eu-1.content-cms.com/api/37dd7bf6-5628-4aac-8464-f4894ddfb8c4 --user adarsh.bhautoo@hangarww.com --password Ad1108bh_hangarMU";
            //ExecuteCommandInApp(initCommand, filePath);

            var commandFile = string.Concat(filePath, "wchtools_non-prod.cmd");
            ExecuteCommandInApp(commandFile, filePath);

        }
    }
}