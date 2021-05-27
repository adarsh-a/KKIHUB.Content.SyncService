using KKIHUB.Content.SyncService.Helper;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KKIHUB.Content.SyncService.Service
{
    public class AcousticService : IAcousticService
    {
        public async Task<JsonObject> FetchArtifactForDateRangeAsync(int days, string hub)
        {
            if (Constants.Constants.HubNameToId.ContainsKey(hub)
                && Constants.Constants.HubToApi.ContainsKey(hub))
            {
                var hubId = Constants.Constants.HubNameToId[hub];
                var hubApi = Constants.Constants.HubToApi[hub];
                try
                {
                    var baseUrl = Constants.Constants.Endpoints.Base.Replace("{hubId}", hubId);
                    var dateRangeUrl = $"{baseUrl}{ Constants.Constants.Endpoints.FetchContentDateRange}";
                    var contentIdUrl = $"{baseUrl}{ Constants.Constants.Endpoints.FetchContentById}";

                    var startdate = DateTime.UtcNow.AddDays(-days).ToString("o");
                    var endDate = DateTime.UtcNow.ToString("o");

                    var parameters = $"start={startdate}&end={endDate}&format=sequence&limit=1";
                    dateRangeUrl = $"{dateRangeUrl}?{parameters}";

                    var request = WebRequest.Create(new Uri(dateRangeUrl));
                    request.Method = "GET";

                    //request.Credentials = new NetworkCredential("AcousticAPIKey", hupApi);

                    string credidentials = "AcousticAPIKey" + ":" + hubApi;
                    var authorization = Convert.ToBase64String(Encoding.Default.GetBytes(credidentials));
                    request.Headers["Authorization"] = "Basic " + authorization;

                    request.ContentType = "application/json";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    using (var response = await request.GetResponseAsync() as HttpWebResponse)
                    {
                        if (response == null)
                        {
                            // not HttpWebResponse
                            return null;
                        }

                        var responseStream = response.GetResponseStream();
                        if (responseStream == null)
                        {
                            // no Response
                            return null;
                        }

                        var reader = new StreamReader(responseStream, Encoding.Default);
                        var responseAsString = reader.ReadToEnd();

                        string[] items = responseAsString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                        if (items != null && items.Any())
                        {
                            foreach (var item in items)
                            {
                                var itemObj = JsonConvert.DeserializeObject<JsonObject>(item);
                                var itemClassification = itemObj["classification"].ToString();
                                var itemId = itemObj["id"];
                                var itemName = $"{itemId}_cmd.json".Replace(":", "_");

                                JsonCreator.CreateJsonFile(itemName, itemClassification, item);

                                ExtractElement(itemObj, contentIdUrl, hubApi);
                            }
                        }

                        return null;
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Fetch Content error : {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Trace.TraceError($"Hub Map not found ");
            }

            return null;
        }


        private async Task FecthContentByIdAsync(string contentIdUrl, List<string> artifactIds, string hubApi)
        {

            foreach (var id in artifactIds)
            {
                try
                {
                    string itemUrl = $"{contentIdUrl}/{id}";

                    var request = WebRequest.Create(new Uri(itemUrl));
                    request.Method = "GET";

                    //request.Credentials = new NetworkCredential("AcousticAPIKey", hupApi);

                    string credidentials = "AcousticAPIKey" + ":" + hubApi;
                    var authorization = Convert.ToBase64String(Encoding.Default.GetBytes(credidentials));
                    request.Headers["Authorization"] = "Basic " + authorization;

                    request.ContentType = "application/json";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    using (var response = await request.GetResponseAsync() as HttpWebResponse)
                    {
                        if (response != null)
                        {
                            var responseStream = response.GetResponseStream();
                            if (responseStream != null)
                            {
                                var reader = new StreamReader(responseStream, Encoding.Default);
                                var responseAsString = reader.ReadToEnd();

                                var itemObj = JsonConvert.DeserializeObject<JsonObject>(responseAsString);
                                var itemClassification = itemObj["classification"].ToString();
                                var itemId = itemObj["id"];
                                var itemName = $"{itemId}_cmd.json".Replace(":", "_");

                                JsonCreator.CreateJsonFile(itemName, itemClassification, responseAsString);

                                ExtractElement(itemObj, contentIdUrl, hubApi);

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Fetch Content error : {ex.Message}");
                }

            }


        }

        private void ExtractElement(JsonObject itemObj, string contentIdUrl, string hubApi)
        {
            var elementString = itemObj["elements"].ToString();
            var itemId = itemObj["id"].ToString();
            List<string> associatedId = new List<string>();

            string[] elements = elementString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            if (elements != null && elements.Any())
            {
                foreach (var element in elements)
                {
                    if (element.Contains("\"id\":"))
                    {
                        var stringSplit = element.Trim().Split(" ");
                        if (stringSplit.Length > 1)
                        {
                            var id = stringSplit[1].Replace("\"", "");
                            if (!associatedId.Contains(id) && !string.Equals(itemId, id))
                            {
                                associatedId.Add(id);
                            }
                        }
                    }
                }
            }

            if (associatedId.Any())
            {
                _ = FecthContentByIdAsync(contentIdUrl, associatedId, hubApi);
            }

        }
    }
}
