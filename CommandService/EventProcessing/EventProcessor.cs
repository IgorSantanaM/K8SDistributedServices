using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor(IServiceProvider serviceProvider, IMapper mapper) : IEventProcessor
    {
        public async Task ProcessEventAsync(string message, CancellationToken cancellationToken)
        {
            var eventType = DetermineEvent(message);
            switch(eventType)
            {
                case EventType.PlatformPublished:
                    await HandlePlatformPublishedAsync(message, cancellationToken);
                    break;
                default:
                    Console.WriteLine("Could not determine the event type");
                    await Task.CompletedTask;
                    break;
            }
        }

        private async Task HandlePlatformPublishedAsync(string message, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(message);

            try
            {
                var plat = mapper.Map<Platform>(platformPublishedDto);
                bool externalPlatformExists = await repository.ExternalPlatformExists(plat.ExternalId, cancellationToken);
                if (externalPlatformExists)
                {
                    Console.WriteLine("Platform already exists...");
                    return;
                }
                await repository.CreatePlatformAsync(plat, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add Platform to DB {ex.Message}");
            }

        }

        private EventType DetermineEvent(string notificationMessage)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            return eventType?.Event switch
            {
                "Platform_Published" => EventType.PlatformPublished,
                _ => EventType.Undetermined
            };
        }

        enum EventType
        {
            PlatformPublished,
             Undetermined    
        }
    }
}
