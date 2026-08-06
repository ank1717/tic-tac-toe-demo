using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) {}

        public DbSet<GameSession> GameSessions => Set<GameSession>();
        public DbSet<MoveLog> MoveLogs => Set<MoveLog>();
        public DbSet<Scoreboard> Scoreboards => Set<Scoreboard>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSession>()
                .HasMany(g => g.Moves)
                .WithOne(m => m.GameSession)
                .HasForeignKey(m => m.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
