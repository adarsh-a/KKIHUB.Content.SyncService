using KKIHUB.Content.SyncService.Service;
using Microsoft.Extensions.DependencyInjection;

namespace KKIHUB.Content.SyncService.Infrastructure.Dependency
{
    public class CoreDependencyRegistry : IDependency
    {
        public void Register(IServiceCollection services)
        {
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<IAcousticService, AcousticService>();

        }
    }
}
