using Ocelot.Configuration;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace ApiGateWay_OCSS.Infrastructure.Ocelot;

public class MyServiceDiscoveryProvider(DownstreamRoute downstreamRoute) : IServiceDiscoveryProvider
{
    private readonly DownstreamRoute _downstreamRoute = downstreamRoute;

    public async Task<List<Service>> GetAsync()
    {
        var services = new List<Service>();
        //...
        //Add service(s) to the list matching the _downstreamRoute
        return services;
    }
}