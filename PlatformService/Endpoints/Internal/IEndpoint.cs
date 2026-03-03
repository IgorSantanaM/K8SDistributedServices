namespace PlatformService.Endpoints.Internal
{
    public interface IEndpoint
    {
        static abstract void DefineEndpoints(WebApplication app);
    }
}
