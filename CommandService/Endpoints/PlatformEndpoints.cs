using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Endpoints.Internal;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Endpoints
{
    public class PlatformEndpoints : IEndpoint
    {
        public static void DefineEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/commands/platforms").WithTags("Platforms Commands"); 
        }

        #region
        private async Task<IResult> GetAllPlatformsAsync([FromServices] IMapper mapper, ICommandRepo repository)
        {
            var platforms = await repository.GetPlatformsAsync(CancellationToken.None);
            var platformReadDtos = mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Results.Ok(platformReadDtos);
        }

        private async Task<IResult> GetAllCommandsForPlatformAsync([FromRoute] int platformId, [FromServices] IMapper mapper, ICommandRepo repository)
        {
            if (!await repository.PlatformExistsAsync(platformId, CancellationToken.None))
                return Results.NotFound();
            var commands = await repository.GetCommandsForPlatformAsync(platformId, CancellationToken.None);
            var commandReadDtos = mapper.Map<IEnumerable<CommandReadDto>>(commands);
            return Results.Ok(commandReadDtos);
        }

        #endregion
    }
}
