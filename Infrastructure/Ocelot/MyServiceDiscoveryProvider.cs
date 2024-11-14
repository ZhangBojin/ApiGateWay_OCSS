using Consul;
using Ocelot.Configuration;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace ApiGateWay_OCSS.Infrastructure.Ocelot;

/// <summary>
/// 自定义服务发现
/// </summary>
/// <param name="downstreamRoute"></param>
public class MyServiceDiscoveryProvider(DownstreamRoute downstreamRoute) : IServiceDiscoveryProvider
{
    private readonly DownstreamRoute _downstreamRoute = downstreamRoute;

    public async Task<List<Service>> GetAsync()
    {
        var consulClient = new ConsulClient(config =>
        {
            config.Address = new Uri("https://4b15480e.r1.cpolar.top");
        });

        var services = new List<Service>();
        var service = await consulClient.Catalog.Service(_downstreamRoute.ServiceName);

        services.Add(new Service(service.Response[0].ServiceName, new ServiceHostAndPort(service.Response[0].ServiceAddress, service.Response[0].ServicePort),
            service.Response[0].ServiceID,null,null));

        return services;
    }
}