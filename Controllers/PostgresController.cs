using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TestBenchWebService.Models;
using TestBenchWebService.Services;

namespace TestBenchWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class PostgresController : ControllerBase
    {
        private readonly PostgresDbContextProvider _provider;
        private readonly IBlogSeeder _seeder;

        public PostgresController(PostgresDbContextProvider provider, IBlogSeeder seeder)
        {
            _provider = provider;
            _seeder = seeder;
        }

        /// <summary>
        /// This method shows all blogs from PostgreSQL
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await TryExecuteAsync(async context =>
            {
                await _seeder.EnsureSeededAsync(context);
                var blogs = await context.Blogs
                    .Where(b => b.Title.Contains("Title"))
                    .Select(c => new { c.Id, c.Title, c.Description })
                    .ToListAsync();
                return Ok(blogs);
            });

            return result;
        }

        /// <summary>
        /// This method shows all blogs by title from PostgreSQL
        /// </summary>
        [HttpGet("{title}")]
        public async Task<IActionResult> GetByTitle(string title)
        {
            var result = await TryExecuteAsync(async context =>
            {
                await _seeder.EnsureSeededAsync(context);
                var blogs = await context.Blogs
                    .Where(b => b.Title == title)
                    .Select(c => new { c.Id, c.Title, c.Description })
                    .ToListAsync();
                return Ok(blogs);
            });

            return result;
        }

        /// <summary>
        /// Test PostgreSQL connection
        /// </summary>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            var result = await TryExecuteAsync(async context =>
            {
                await context.Database.CanConnectAsync();
                return Ok(new { Status = "Connected", Database = "PostgreSQL" });
            });

            return result;
        }

        private async Task<IActionResult> TryExecuteAsync(Func<ApiDbContext, Task<IActionResult>> operation)
        {
            // Attempt #1
            await using (var context = _provider.Create())
            {
                if (context is null)
                {
                    return BadRequest(new
                    {
                        Status = "Not Configured",
                        Error = "PostgreSQL is not configured. Please provide ConnectionStrings:PostgreSQL.",
                        Database = "PostgreSQL"
                    });
                }

                try
                {
                    return await operation(context);
                }
                catch
                {
                    // fall through to retry
                }
            }

            // Attempt #2 (after config reload in provider)
            await using (var context2 = _provider.Create())
            {
                if (context2 is null)
                {
                    return BadRequest(new
                    {
                        Status = "Not Configured",
                        Error = "PostgreSQL is not configured. Please provide ConnectionStrings:PostgreSQL.",
                        Database = "PostgreSQL"
                    });
                }

                try
                {
                    return await operation(context2);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Status = "Failed", Error = ex.Message, Database = "PostgreSQL" });
                }
            }
        }
    }
}


