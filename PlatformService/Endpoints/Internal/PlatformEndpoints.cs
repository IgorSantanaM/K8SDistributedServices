using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;

namespace PlatformService.Endpoints.Internal
{
    public class PlatformEndpoints : IEndpoint
    {
        public static void DefineEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/platforms").WithTags("Platforms");

            app.MapPost("/", CreatePlatform)
                .WithName("CreatePlatform")
                .WithDescription("Creates a new platform")
                .Accepts<PlatformCreateDto>("application/json")
                .Produces(201)
                .Produces(400);

            app.MapGet("/", GetPlatforms)
                .WithName("GetPlatforms")
                .WithDescription("Gets all platforms")
                .Produces<IEnumerable<PlatformReadDto>>(200);
        }


        #region Handlers

        private static async Task<IResult> GetPlatforms(IPlatformRepository repository, IMapper mapper)
        {
            var platforms = await repository.GetAllPlatformsAsync();
            var platformDtos = mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Results.Ok(platformDtos);
        }
        private static async Task<IResult> CreatePlatform([FromBody] PlatformCreateDto platformCreateDto, IPlatformRepository repository, IMapper mapper)
        {
           var platformModel = mapper.Map<Models.Platform>(platformCreateDto);
            await repository.CreatePlatformAsync(platformModel);
            return Results.Created();
        }

        #endregion
    }
}
