using DevAssistAI.Entites;
using Microsoft.EntityFrameworkCore;

namespace DevAssistAI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessages> ChatMessages { get; set; }

        protected override void OnModelCreating( ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChatMessages>()
                .HasOne(m => m.ChatSession)
                .WithMany()
                .HasForeignKey(m => m.ChatSessionId);
        }
    }
}
