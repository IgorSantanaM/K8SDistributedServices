namespace CommandService.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEventAsync(string message, CancellationToken cancellationToken);

    }
}
