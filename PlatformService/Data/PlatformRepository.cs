using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository(AppDbContext dbContext) : IPlatformRepository
    {
        public async Task CreatePlatformAsync(Platform plat, CancellationToken cancellationToken = default)
        {
            if(plat is null)
                throw new ArgumentNullException(nameof(plat));
            await dbContext.AddAsync(plat, cancellationToken);
        }

        public async Task<IEnumerable<Platform>?> GetAllPlatformsAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Platforms.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Platform?> GetPlatformByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Platforms.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken: cancellationToken) >= 0;
        }
    }
}
