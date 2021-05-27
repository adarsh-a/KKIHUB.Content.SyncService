using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KKIHUB.Content.SyncService.Service
{
    public class ContentService : IContentService
    {
        private IAcousticService acousticService;
        public ContentService(IAcousticService acousticService)
        {
            this.acousticService = acousticService;
        }

        public async Task<JsonObject> FetchContentAsync(int days, string hubId)
        {
            var artifacts = await acousticService.FetchArtifactForDateRangeAsync(days, hubId);
            try
            {
                return string.IsNullOrWhiteSpace(artifacts.ToString()) ? null : artifacts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Fetch Artifacts error : {ex.Message}");
                return null;
            }

        }
    }
}
