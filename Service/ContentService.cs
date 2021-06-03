using KKIHUB.Content.SyncService.Model;
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

        public async Task<List<ContentModel>> FetchContentAsync(int days, string hubId, bool recursive, bool onlyUpdated)
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
        
        public async Task<List<string>> FetchTypeAsync(int days, string hubId, bool recursive, bool onlyUpdated)
        {
            try
            {
                var artifacts = await acousticService.FetchTypeAsync(days, hubId, recursive, onlyUpdated);
                return artifacts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Fetch Artifacts error : {ex.Message}");
                return null;
            }

        }


        public async Task<List<ContentModel>> FetchContentByLibrary(string hubId, string libraryId)
        {
            try
            {
                var artifacts = await acousticService.FetchContentByLibrary(hubId, libraryId);
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
