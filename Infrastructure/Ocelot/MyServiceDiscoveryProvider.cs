using Consul;
using Ocelot.Configuration;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace ApiGateWay_OCSS.Infrastructure.Ocelot;

/// <summary>
/// 自定义服务发现
/// </summary>
/// <param name="downstreamRoute"></param>
public class MyServiceDiscoveryProvider(DownstreamRoute downstreamRoute, IConfiguration configuration) : IServiceDiscoveryProvider
{
    private readonly DownstreamRoute _downstreamRoute = downstreamRoute;
    private readonly IConfiguration _configuration = configuration;

    public async Task<List<Service>> GetAsync()
    {
        var consulClient = new ConsulClient(config =>
        {
            config.Address = new Uri($"{_configuration.GetSection("Consul")["Address"]}");
        });

        var service = await consulClient.Catalog.Service(_downstreamRoute.ServiceName);

        return service.Response.Select(entry => new Service(entry.ServiceName, new ServiceHostAndPort(entry.ServiceAddress, entry.ServicePort), entry.ServiceID, "null", entry.ServiceTags)).ToList();
    }
}