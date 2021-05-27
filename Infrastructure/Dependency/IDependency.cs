using Microsoft.Extensions.DependencyInjection;

namespace KKIHUB.Content.SyncService.Infrastructure.Dependency
{
    public interface IDependency
    {
        void Register(IServiceCollection services);
    }
}
