using AdjusterOptimizerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdjusterOptimizerAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Adjuster> Adjusters { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<PerformanceHistory> PerformanceHistory { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary Keys
            modelBuilder.Entity<Claim>().HasKey(c => c.ClaimId);
            modelBuilder.Entity<Adjuster>().HasKey(a => a.AdjusterId);
            modelBuilder.Entity<Assignment>().HasKey(a => a.AssignmentId);
            modelBuilder.Entity<PerformanceHistory>().HasKey(p => p.RecordId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            // Assignment → Claim (many-to-one)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Claim)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.ClaimId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment → Adjuster (many-to-one)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Adjuster)
                .WithMany(ad => ad.Assignments)
                .HasForeignKey(a => a.AdjusterId)
                .OnDelete(DeleteBehavior.Restrict);

            // PerformanceHistory → Adjuster (many-to-one)
            modelBuilder.Entity<PerformanceHistory>()
                .HasOne(p => p.Adjuster)
                .WithMany(a => a.PerformanceHistory)
                .HasForeignKey(p => p.AdjusterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Claim → AssignedAdjuster (optional FK)
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.AssignedAdjuster)
                .WithMany()
                .HasForeignKey(c => c.AssignedAdjusterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
