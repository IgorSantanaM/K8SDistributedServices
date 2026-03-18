using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.SyncDataServices.Http;

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

            app.MapGet("/{id:int}", GetPlatformById)
                .WithName("GetPlatformById")
                .WithDescription("Gets a platform by ID")
                .Produces<PlatformReadDto>(200)
                .Produces(404);
        }


        #region Handlers

        private static async Task<IResult> GetPlatformById([FromRoute] int id, IPlatformRepository repository, 
            IMapper mapper)
        {
            var platform = await repository.GetPlatformByIdAsync(id);
            if (platform == null)
            {
                return Results.NotFound();
            }
            var platformDto = mapper.Map<PlatformReadDto>(platform);
            return Results.Ok(platformDto);
        }
        private static async Task<IResult> GetPlatforms(IPlatformRepository repository, 
            IMapper mapper)
        {
            var platforms = await repository.GetAllPlatformsAsync();
            var platformDtos = mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Results.Ok(platformDtos);
        }
        private static async Task<IResult> CreatePlatform([FromBody] PlatformCreateDto platformCreateDto,
            IPlatformRepository repository, 
            IMapper mapper,
            IMessageBusClient messageBusClient)
        {
           var platformModel = mapper.Map<Models.Platform>(platformCreateDto);
            await repository.CreatePlatformAsync(platformModel);
            if(await repository.SaveChanges())
                return Results.Created($"/api/platforms/{platformModel.Id}", platformModel);
            var platformReadDto = mapper.Map<PlatformReadDto>(platformModel);
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);   
            platformPublishedDto.Event = "Platform_Published";
            try
            {
                await messageBusClient.PublishNewPlatformAsync(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            return Results.BadRequest();
        }

        #endregion
    }
}
