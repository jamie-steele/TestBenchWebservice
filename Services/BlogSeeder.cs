using Microsoft.EntityFrameworkCore;
using TestBenchWebService.Models;

namespace TestBenchWebService.Services
{
    public interface IBlogSeeder
    {
        Task EnsureSeededAsync(DbContext context, CancellationToken cancellationToken = default);
    }

    public sealed class BlogSeeder : IBlogSeeder
    {
        public async Task EnsureSeededAsync(DbContext context, CancellationToken cancellationToken = default)
        {
            await context.Database.EnsureCreatedAsync(cancellationToken);

            var blogSet = context.Set<Blog>();
            if (await blogSet.AnyAsync(cancellationToken))
            {
                return;
            }

            var blogs = new List<Blog>
            {
                new() { Title = "Title1", Description = "Description for Title1" },
                new() { Title = "Title2", Description = "Description for Title2" },
                new() { Title = "Title3", Description = "Description for Title3" },
                new() { Title = "Test Blog", Description = "This is a test blog entry" },
                new() { Title = "Sample Post", Description = "A sample blog post for testing" }
            };

            await blogSet.AddRangeAsync(blogs, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}


