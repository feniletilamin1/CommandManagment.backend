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
        public DbSet<Board> ScrumBoards { get; set; }
        public DbSet<BoardColumn> ScrumBoardColumns { get; set; }
        public DbSet<BoardTask> ScrumBoardTasks { get; set; }
        public DbSet<InviteToken> InviteTokens { get; set; }

        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<BoardTask>()
                .HasOne(u => u.ScrumBoardColumn)
                .WithMany(c => c.ScrumBoardTasks)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardTask>()
              .HasOne(u => u.ResponsibleUser)
              .WithMany(c => c.UserTasks)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BoardTask>()
            .HasOne(u => u.CreateUserTask)
            .WithMany(c => c.CreatedTasks)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Project>()
               .HasOne(e => e.Board)
                    .WithOne(e => e.Project)
               .HasForeignKey<Board>(e => e.ProjectId)
               .IsRequired();
        }
    }
}