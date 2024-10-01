using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Identity_Auth.Models;

namespace Identity_Auth.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UserDatabaseConnection> UserDatabaseConnections { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<Graphic> Graphics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDatabaseConnection>()
                .HasOne(udc => udc.User)
                .WithMany()
                .HasForeignKey(udc => udc.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserReport>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dashboard>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Graphic>()
                .HasOne(g => g.Dashboard)
                .WithMany(d => d.Graphics)
                .HasForeignKey(g => g.DashboardId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Graphic>()
                .HasOne(g => g.Connection)
                .WithMany()
                .HasForeignKey(g => g.ConnectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
