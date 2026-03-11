using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    {
        // Platforms
        Task<IEnumerable<Platform>> GetPlatformsAsync(CancellationToken cancellationToken);
        Task CreatePlatformAsync(Platform platform, CancellationToken cancellationToken);   
        Task<bool> PlatformExistsAsync(int platformId, CancellationToken cancellationToken);

        // Commands
        Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId, CancellationToken cancellationToken);
        Task<Command?> GetCommandAsync(int platformId, int commandId, CancellationToken cancellationToken);
        Task CreateCommandAsync(int platformId, Command command, CancellationToken cancellationToken);

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
