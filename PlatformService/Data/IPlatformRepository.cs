using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepository
    {
        Task<bool> SaveChanges(CancellationToken cancellationToken = default);

        Task<IEnumerable<Platform>?> GetAllPlatformsAsync(CancellationToken cancellationToken = default);
        Task<Platform?> GetPlatformByIdAsync(int id, CancellationToken cancellationToken = default);
        Task CreatePlatformAsync(Platform plat, CancellationToken cancellationToken = default);
    }
}
