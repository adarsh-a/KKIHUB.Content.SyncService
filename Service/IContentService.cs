using KKIHUB.Content.SyncService.Model;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KKIHUB.Content.SyncService.Service
{
    public interface IContentService
    {
        Task<List<ContentModel>> FetchContentAsync(int days, string hubId, bool recursive, bool onlyUpdated);


        Task<List<string>> FetchTypeAsync(int days, string hubId, bool recursive, bool onlyUpdated);


        Task<List<ContentModel>> FetchContentByLibrary(string hubId, string libraryId);



    }
}
