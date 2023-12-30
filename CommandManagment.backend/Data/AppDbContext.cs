using Azure.Core.Pipeline;
using CommandManagment.backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandManagment.backend.Data
{
    public class AppDbContext: DbContext
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

            modelBuilder.Entity<ScrumBoard>()
                .HasOne(sb => sb.User)
                .WithMany(u => u.ScrumBoards)
                .HasForeignKey(sb => sb.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ScrumBoardTask>()
                   .HasOne(sb => sb.ScrumBoardColumn)
                   .WithMany(u => u.ScrumBoardTasks)
                   .HasForeignKey(sb => sb.ScrumBoardColumnId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
