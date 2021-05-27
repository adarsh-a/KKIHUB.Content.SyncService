using RestSharp;
using System.Threading.Tasks;

namespace KKIHUB.Content.SyncService.Service
{
    public interface IContentService
    {
        Task<JsonObject> FetchContentAsync(int days, string hubId);
    }
}
