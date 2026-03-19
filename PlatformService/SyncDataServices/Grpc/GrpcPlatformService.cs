using AutoMapper;
using Grpc.Core;
using PlatformService.Data;
using static PlatformService.GrpcPlatform;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper) : GrpcPlatformBase
    {
        public override async Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        { 
        
            var response = new PlatformResponse();
            var platforms = await platformRepository.GetAllPlatformsAsync();
            
            foreach ( var platform in platforms!)
            {
                response.Platform.Add(mapper.Map<GrpcPlatformModel>(platform));
            }
            return response;
        }
    }
}
