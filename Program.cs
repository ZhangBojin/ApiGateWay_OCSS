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

#region ocelot ����
//������Ϊ ocelot �������ļ�
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//ע��ServiceDiscoveryFinderDelegate �Գ�ʼ���������ṩ����
ServiceDiscoveryFinderDelegate serviceDiscoveryFinder = (provider, config, route) => new MyServiceDiscoveryProvider(route, provider.GetRequiredService<IConfiguration>());
builder.Services.AddSingleton(serviceDiscoveryFinder);
builder.Services.AddOcelot();
#endregion

#region ���ݿ�������scopedע��
builder.Services.AddDbContext<OCSS_DbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<LogServiceDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("LogServiceConn"));
});
#endregion

#region �ִ�����ע��
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IMenuInfoRepository, MenuInfoRepository>();
#endregion 

#region ����Jwt
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //��Ĭ�ϵ������֤����ʧ��ʱ��Ӧ�ý�ʹ���������������ս
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

#region ������־��Ϣ����
    builder.Services.AddSingleton<RabbitMqProducer>();
#endregion

#region Redis����
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
        return ConnectionMultiplexer.Connect(configuration!);
    });
#endregion

#region ����Consul
    builder.Services.AddSingleton<IConsulClient, ConsulClient>(client =>
    {
        return new ConsulClient(config =>
        {
            config.Address = new Uri(builder.Configuration.GetSection("Consul")["Address"]!); // Consul server��ַ
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
    //// ֱ��ӳ�䵽 ASP.NET Core ������
    _ = endpoints.MapControllers();
});

app.UseOcelot().Wait();

app.Run();
