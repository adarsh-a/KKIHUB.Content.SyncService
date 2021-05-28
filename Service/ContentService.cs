using Newtonsoft.Json;
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

        public async Task<List<string>> FetchContentAsync(int days, string hubId, bool recursive, bool onlyUpdated)
        {
            try
            {
                var artifacts = await acousticService.FetchArtifactForDateRangeAsync(days, hubId, recursive, onlyUpdated);
                return artifacts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Fetch Artifacts error : {ex.Message}");
                return null;
            }

        }
        
        public async Task<List<string>> FetchAssetAsync(int days, string hubId, bool recursive, bool onlyUpdated)
        {
            try
            {
                var artifacts = await acousticService.FetchAssetForDateRangeAsync(days, hubId, recursive, onlyUpdated);
                return artifacts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Fetch Artifacts error : {ex.Message}");
                return null;
            }

        }
    }
}
