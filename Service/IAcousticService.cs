using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KKIHUB.Content.SyncService.Service
{
    public interface IAcousticService
    {
        Task<JsonObject> FetchArtifactForDateRangeAsync(int days, string hub);
    }
}
