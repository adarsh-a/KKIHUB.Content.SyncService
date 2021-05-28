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
        private List<string> ItemsFetched = new List<string>();

        public async Task<List<string>> FetchArtifactForDateRangeAsync(int days, string hub, bool recursive, bool onlyUpdated)
        {
            ItemsFetched.Add($"Sync Started at {DateTime.Now}");
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

                    var parameters = $"start={startdate}&end={endDate}&format=sequence";
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
                        if (response != null)
                        {
                            await ResponseStreamLogicAsync(response, contentIdUrl, hubApi, recursive, startdate, onlyUpdated);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Fetch Content error : {ex.Message}");
                    return ItemsFetched;
                }
            }
            else
            {
                System.Diagnostics.Trace.TraceError($"Hub Map not found ");
                return ItemsFetched;
            }
            ItemsFetched.Add($"Sync Ended at {DateTime.Now}");
            return ItemsFetched;
        }

        private async Task FecthContentByIdAsync(string contentIdUrl, List<string> artifactIds, string hubApi, bool recursive, string startDate)
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
                                var lastModifiedDate = itemObj["lastModified"].ToString();

                                if (ShouldUpdate(lastModifiedDate, startDate, recursive))
                                {
                                    var msg = JsonCreator.CreateJsonFile(itemName, itemClassification, responseAsString);
                                    if (!string.IsNullOrWhiteSpace(msg))
                                    {
                                        ItemsFetched.Add(msg);
                                    }
                                }

                                await ExtractElementAsync(itemObj, contentIdUrl, hubApi, recursive, startDate);

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = $"Fetch Content with id {id} error : {ex.Message} ";
                    ItemsFetched.Add(errorMsg);

                    System.Diagnostics.Trace.TraceError($"Fetch Content error : {ex.Message}");

                }
            }
        }

        private async Task ExtractElementAsync(JsonObject itemObj, string contentIdUrl, string hubApi, bool recursive, string startDate)
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
                await FecthContentByIdAsync(contentIdUrl, associatedId, hubApi, recursive, startDate);
            }

        }


        private bool ShouldUpdate(string lastModifiedDate, string startDate, bool recursive)
        {
            if (!recursive)
            {
                DateTime strtDate = DateTime.Parse(startDate).Date;
                DateTime modifiedDate = DateTime.Parse(lastModifiedDate).Date;
                if (modifiedDate > strtDate) return true;
                return false;

            }
            return true;

        }


        public async Task<List<string>> FetchAssetForDateRangeAsync(int days, string hub, bool recursive, bool onlyUpdated)
        {
            ItemsFetched.Add($"Sync Started at {DateTime.Now}");
            if (Constants.Constants.HubNameToId.ContainsKey(hub)
                && Constants.Constants.HubToApi.ContainsKey(hub))
            {
                var hubId = Constants.Constants.HubNameToId[hub];
                var hubApi = Constants.Constants.HubToApi[hub];
                try
                {
                    var baseUrl = Constants.Constants.Endpoints.Base.Replace("{hubId}", hubId);
                    var dateRangeUrl = $"{baseUrl}{ Constants.Constants.Endpoints.FetchAssetDateRange}";

                    var startdate = DateTime.UtcNow.AddDays(-days).ToString("o");
                    var endDate = DateTime.UtcNow.ToString("o");

                    var parameters = $"start={startdate}&end={endDate}&format=sequence";
                    dateRangeUrl = $"{dateRangeUrl}?{parameters}";

                    var request = WebRequest.Create(new Uri(dateRangeUrl));
                    request.Method = "GET";

                    string credidentials = "AcousticAPIKey" + ":" + hubApi;
                    var authorization = Convert.ToBase64String(Encoding.Default.GetBytes(credidentials));
                    request.Headers["Authorization"] = "Basic " + authorization;

                    request.ContentType = "application/json";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    using (var response = await request.GetResponseAsync() as HttpWebResponse)
                    {
                        if (response != null)
                        {
                            //await ResponseStreamLogicAsync(response, string.Empty, hubApi, recursive, startdate, onlyUpdated, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Fetch Content error : {ex.Message}");
                    return ItemsFetched;
                }
            }
            else
            {
                System.Diagnostics.Trace.TraceError($"Hub Map not found ");
                return ItemsFetched;
            }
            ItemsFetched.Add($"Sync Ended at {DateTime.Now}");
            return ItemsFetched;
        }


        private async Task ResponseStreamLogicAsync(HttpWebResponse response,
            string contentIdUrl, string hubApi, bool recursive,
            string startdate, bool onlyUpdated, bool isAsset = false)
        {

            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
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

                        var msg = JsonCreator.CreateJsonFile(itemName, itemClassification, item);
                        if (!string.IsNullOrWhiteSpace(msg))
                        {
                            ItemsFetched.Add(msg);

                        }

                        if (!isAsset && !onlyUpdated)
                        {
                            await ExtractElementAsync(itemObj, contentIdUrl, hubApi, recursive, startdate);
                        }
                    }
                }
            }
        }


        private async Task ExtractElementAsyncv2(JsonObject itemObj, string contentIdUrl, string hubApi, bool recursive, string startDate)
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
                await FecthContentByIdAsync(contentIdUrl, associatedId, hubApi, recursive, startDate);
            }

        }




    }
}
