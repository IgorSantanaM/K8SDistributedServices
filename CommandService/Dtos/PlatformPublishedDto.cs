namespace CommandService.Dtos
{
    public record PlatformPublishedDto(int Id, string Name)
    {
        public string Event { get; set; }
    };
}
