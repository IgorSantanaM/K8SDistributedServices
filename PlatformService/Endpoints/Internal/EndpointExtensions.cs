using System.Reflection;

namespace PlatformService.Endpoints.Internal
{
    public static class EndpointExtensions
    {
        public static void UseEndpoints<TMarker>(this IApplicationBuilder app)
            => UseEndpoints(app, typeof(TMarker));

        private static void UseEndpoints(IApplicationBuilder app, Type type)
        {
            IEnumerable<TypeInfo> endpointType = GetEndpointTypes(type);

            foreach (TypeInfo typeInfo in endpointType)
                typeInfo.GetMethod(nameof(IEndpoint.DefineEndpoints))?
                    .Invoke(null, [app]);
        }

        private static IEnumerable<TypeInfo> GetEndpointTypes(Type type)
        {
            return type.Assembly.DefinedTypes.Where
                (x => !x.IsAbstract && !x.IsInterface && typeof(IEndpoint).IsAssignableFrom(x));
        }
    }
}
