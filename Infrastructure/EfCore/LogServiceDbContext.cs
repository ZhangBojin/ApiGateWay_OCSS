using ApiGateWay_OCSS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiGateWay_OCSS.Infrastructure.EfCore;

public class LogServiceDbContext(DbContextOptions<LogServiceDbContext> options) : DbContext(options)
{
    public virtual DbSet<Log> Log { get; set; }
}