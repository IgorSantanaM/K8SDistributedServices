using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Endpoints.Internal;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Endpoints
{
    public class PlatformEndpoints : IEndpoint
    {
        public static void DefineEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/commands/platforms").WithTags("Platforms Commands"); 
            
            group.MapGet("/", GetAllPlatformsAsync);
            group.MapGet("/{platformId}/commands", GetAllCommandsForPlatformAsync);
            group.MapGet("/{platformId}/commands/{commandId}", GetCommandForPlatformAsync);
            group.MapPost("/{platformId}/commands", CreateCommandForPlatformAsync);
        }

        #region
        private static async Task<IResult> GetAllPlatformsAsync([FromServices] IMapper mapper, ICommandRepo repository)
        {
            var platforms = await repository.GetPlatformsAsync(CancellationToken.None);
            var platformReadDtos = mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Results.Ok(platformReadDtos);
        }

        private static async Task<IResult> GetAllCommandsForPlatformAsync([FromRoute] int platformId, [FromServices] IMapper mapper, ICommandRepo repository)
        {
            if (!await repository.PlatformExistsAsync(platformId, CancellationToken.None))
                return Results.NotFound();
            var commands = await repository.GetCommandsForPlatformAsync(platformId, CancellationToken.None);
            var commandReadDtos = mapper.Map<IEnumerable<CommandReadDto>>(commands);
            return Results.Ok(commandReadDtos);
        }

        private static async Task<IResult> GetCommandForPlatformAsync([FromRoute] int platformId,
            [FromRoute] int commandId, 
            [FromServices] IMapper mapper, 
            ICommandRepo repository)
        {
            var command = await repository.GetCommandAsync(platformId, commandId, CancellationToken.None);
            if (command is null)
                return Results.NotFound();
            var commandReadDto = mapper.Map<CommandReadDto>(command);
            return Results.Ok(commandReadDto);
        }
        private static async Task<IResult> CreateCommandForPlatformAsync([FromRoute] int platformId,
            [FromBody] CommandCreateDto commandCreateDto,
            [FromServices] IMapper mapper,
            ICommandRepo repository)
        {
            if (!await repository.PlatformExistsAsync(platformId, CancellationToken.None))
                return Results.NotFound();
            var command = mapper.Map<Command>(commandCreateDto);
            await repository.CreateCommandAsync(platformId, command, CancellationToken.None);
            await repository.SaveChangesAsync(CancellationToken.None);
            var commandReadDto = mapper.Map<CommandReadDto>(command);
            return Results.Created($"/api/commands/platforms/{platformId}/commands/{command.Id}", commandReadDto);
        }
        #endregion
    }
}
