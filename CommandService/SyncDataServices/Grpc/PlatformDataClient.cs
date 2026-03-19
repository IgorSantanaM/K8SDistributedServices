using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient(IConfiguration configuration, IMapper mapper) : IPlatformDataClient
    {
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var channel = GrpcChannel.ForAddress(configuration["GrpcPlatform"!]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Couldn't reach the grpc server: " + ex.Message);
                return null!;
            }
        }
    }
}
