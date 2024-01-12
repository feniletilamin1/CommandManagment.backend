using Azure.Core.Pipeline;
using CommandManagment.backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandManagment.backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<InviteTeamToken> Invites { get; set; }
        public DbSet<ScrumBoard> ScrumBoards { get; set; }
        public DbSet<ScrumBoardColumn> ScrumBoardColumns { get; set; }
        public DbSet<ScrumBoardTask> ScrumBoardTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<ScrumBoardTask>()
            .HasOne(u => u.ScrumBoardColumn)
            .WithMany(c => c.ScrumBoardTasks)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}