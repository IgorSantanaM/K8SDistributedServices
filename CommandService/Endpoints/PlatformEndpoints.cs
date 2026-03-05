using CommandService.Endpoints.Internal;

namespace CommandService.Endpoints
{
    public class PlatformEndpoints : IEndpoint
    {
        public static void DefineEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/commands/platforms").WithTags("Platforms Commands"); 
        }

        #region

        #endregion
    }
}
