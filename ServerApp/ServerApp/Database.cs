using Microsoft.EntityFrameworkCore;

namespace ServerApp
{
    public class Database : DbContext
    {
        public DbSet<Tables.User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                $"server=176.9.66.40;database=sr;user=sr_app;password=wTG!9n4cY7_tx.Q!;SslMode=none;Convert Zero Datetime=true;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}