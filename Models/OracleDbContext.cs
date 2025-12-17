using Microsoft.EntityFrameworkCore;
using TestBenchWebService.Maps;

namespace TestBenchWebService.Models
{
    public sealed class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
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


