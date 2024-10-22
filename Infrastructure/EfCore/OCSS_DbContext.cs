using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApiGateWay_OCSS.Infrastructure.EfCore
{
    public class OCSS_DbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Users?> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Teachers> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //设置复合主键
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>().HasOne<Users>().WithMany().HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>().HasOne<Roles>().WithMany().HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasNoKey(); // 视图没有主键
                entity.ToView("UserInfo"); // 指定视图名称
            });

            modelBuilder.Entity<Students>(entity =>
            {
                // 设置 StudentId 为主键，不自增长
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.StudentId)
                    .ValueGeneratedNever(); // 确保不自增长

                // 设置 UserId 为外键，且唯一
                entity.HasOne<Users>() // 假设 User 是你的用户实体
                    .WithOne() // 一个用户对应一个学生
                    .HasForeignKey<Students>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // 级联删除

                // 确保 UserId 唯一
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            modelBuilder.Entity<Teachers>(entity =>
            {
                entity.HasKey(e => e.TeacherId);
                entity.Property(e => e.TeacherId)
                    .ValueGeneratedNever();

                entity.HasOne<Users>()
                    .WithOne()
                    .HasForeignKey<Teachers>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId).IsUnique();
            });
        }
    }
}
