using Microsoft.EntityFrameworkCore;
using TestBenchWebService.Maps;

namespace TestBenchWebService.Models
{
    public sealed class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs => Set<Blog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ = new BlogMap(modelBuilder.Entity<Blog>());
        }
    }
}


