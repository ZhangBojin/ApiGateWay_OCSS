using ApiGateWay_OCSS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiGateWay_OCSS.Infrastructure.EfCore
{
    public class OCSS_DbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Users?> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //设置复合主键
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>().HasOne<Users>().WithMany().HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>().HasOne<Roles>().WithMany().HasForeignKey(ur => ur.RoleId);
        }
    }
}
