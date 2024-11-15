using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using ApiGateWay_OCSS.Infrastructure.Ocelot;
using ApiGateWay_OCSS.Infrastructure.RabbitMq;
using ApiGateWay_OCSS.Infrastructure.Repositories;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.ServiceDiscovery;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region ocelot 配置
//加载名为 ocelot 的配置文件
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//注册ServiceDiscoveryFinderDelegate 以初始化并返回提供程序
ServiceDiscoveryFinderDelegate serviceDiscoveryFinder = (provider, config, route) => new MyServiceDiscoveryProvider(route, provider.GetRequiredService<IConfiguration>());
builder.Services.AddSingleton(serviceDiscoveryFinder);
builder.Services.AddOcelot();
#endregion

#region 数据库上下文scoped注入
builder.Services.AddDbContext<OCSS_DbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<LogServiceDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("LogServiceConn"));
});
#endregion

#region 仓储服务注入
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IMenuInfoRepository, MenuInfoRepository>();
#endregion 

#region 配置Jwt
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //当默认的身份验证方案失败时，应用将使用这个方案发起挑战
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("JwtSettings")["Issuer"],
        ValidAudience = builder.Configuration.GetSection("JwtSettings")["Audience"],
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["SecretKey"]!))
    };
});
#endregion

#region 配置日志消息队列
    builder.Services.AddSingleton<RabbitMqProducer>();
#endregion

#region Redis配置
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
        return ConnectionMultiplexer.Connect(configuration!);
    });
#endregion

#region 配置Consul
    builder.Services.AddSingleton<IConsulClient, ConsulClient>(client =>
    {
        return new ConsulClient(config =>
        {
            config.Address = new Uri(builder.Configuration.GetSection("Consul")["Address"]!); // Consul server地址
        });
    });
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    //// 直接映射到 ASP.NET Core 控制器
    _ = endpoints.MapControllers();
});

app.UseOcelot().Wait();

app.Run();
