using System.Text.Json.Serialization;

namespace PlatformService.Dtos
{
    public record PlatformPublishedDto(int Id, string Name)
    {
        public string Event { get; set; }
    };
}
