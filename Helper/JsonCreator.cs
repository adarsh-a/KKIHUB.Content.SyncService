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
        public static string CreateJsonFile(string name, string type, object details)
        {
            string path = string.Concat(Constants.Constants.Path.ArtifactPath, type);
            Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, name);

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, details.ToString());
                var msg = $"Item with Id {name} created";
                Console.WriteLine(msg);
                return msg;
            }
            return string.Empty;
        }
    }
}
