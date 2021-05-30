using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KKIHUB.Content.SyncService.Service
{
    public interface IAcousticService
    {
        Task<List<string>> FetchArtifactForDateRangeAsync(int days, string hub, bool recursive, bool onlyUpdated);

        Task<List<string>> FetchTypeAsync(int days, string hub, bool recursive, bool onlyUpdated);


        Task<List<string>> FetchContentByLibrary(string hub, string libraryId);


    }
}
