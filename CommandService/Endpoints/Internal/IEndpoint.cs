namespace CommandService.Endpoints.Internal
{
    public interface IEndpoint
    {
        public abstract static void DefineEndpoints(WebApplication app);
    }
}
