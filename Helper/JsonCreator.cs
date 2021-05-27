using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KKIHUB.Content.SyncService.Helper
{
    public static class JsonCreator
    {
        public static void CreateJsonFile(string name, string type, object details)
        {
            string path = string.Concat(Constants.Constants.Path.ArtifactPath, type);
            Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, name);

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, details.ToString());

            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", filePath);
                return;
            }

        }
    }
}
