using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using ApiGateWay_OCSS.Infrastructure.RabbitMq;
using ApiGateWay_OCSS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    builder.Services.AddScoped<RabbitMqProducer>();
#endregion

#region Redis配置
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
        return ConnectionMultiplexer.Connect(configuration!);
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
app.UseAuthorization();

app.MapControllers();

app.Run();
