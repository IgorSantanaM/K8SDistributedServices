using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class CommandRepo(AppDbContext dbContext) : ICommandRepo
    {
        public async Task CreateCommandAsync(int platformId, Command command, CancellationToken cancellationToken)
        {
            if(platformId == 0) 
                throw new ArgumentException("PlatformId cannot be zero.", nameof(platformId));
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            await dbContext.AddAsync(command, cancellationToken);
         }

        public async Task CreatePlatformAsync(Platform platform, CancellationToken cancellationToken)
            {
            if(platform == null)
                throw new ArgumentNullException(nameof(platform));
            await dbContext.AddAsync(platform, cancellationToken);
        }

        public Task<Command?> GetCommandAsync(int platformId, int commandId, CancellationToken cancellationToken)
            => dbContext.Commands.AsNoTracking()
                .FirstOrDefaultAsync(c => c.PlatformId == platformId && c.Id == commandId, cancellationToken);

        public async Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId, CancellationToken cancellationToken)
            => await dbContext.Commands.AsNoTracking()
                .Where(c => c.PlatformId == platformId)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Platform>> GetPlatformsAsync(CancellationToken cancellationToken)
            => await dbContext.Platforms.AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<bool> PlatformExistsAsync(int platformId, CancellationToken cancellationToken)
            => await dbContext.Platforms.AsNoTracking()
                .AnyAsync(p => p.Id == platformId, cancellationToken);  

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
            => await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
