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
                var hupApi = Constants.Constants.HubToApi[hub];
                try
                {
                    var baseUrl = Constants.Constants.Endpoints.Base.Replace("{hubId}", hubId);
                    var dateRangeUrl = $"{baseUrl}{ Constants.Constants.Endpoints.FetchContentDateRange}";
                    var contentIdUrl = $"{baseUrl}{ Constants.Constants.Endpoints.FetchContentById}";

                    var startdate = DateTime.UtcNow.AddDays(-days).ToString("o");
                    var endDate = DateTime.UtcNow.ToString("o");

                    var parameters = $"start={startdate}&end={endDate}";
                    dateRangeUrl = $"{dateRangeUrl}?{parameters}";

                    var request = WebRequest.Create(new Uri(dateRangeUrl));
                    request.Method = "GET";

                    //request.Credentials = new NetworkCredential("AcousticAPIKey", hupApi);

                    string credidentials = "AcousticAPIKey" + ":" + hupApi;
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

                        var resp = JsonConvert.DeserializeObject<JsonObject>(responseAsString);

                        var items = resp["items"] as List<JObject>;

                        if (items != null)
                        {
                            int itemsCount = items.Count;
                            //foreach (var item in items) 
                            //{
                            JsonCreator.CreateJsonFile("test.json", "content", "test");
                            //}

                        }

                        return resp;
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


        private void FecthContentById(string url, string id, string apiKey)
        {

        }
    }
}
