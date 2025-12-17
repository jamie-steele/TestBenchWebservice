using Microsoft.AspNetCore.Mvc;
using TestBenchWebService.Models;

namespace TestBenchWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class SnowflakeController : ControllerBase
    {
        private readonly SnowflakeService _snowflakeService;

        public SnowflakeController(SnowflakeService snowflakeService)
        {
            _snowflakeService = snowflakeService;
        }

        /// <summary>
        /// This method shows all blogs from Snowflake
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await TryExecuteAsync(async () =>
            {
                var blogs = await _snowflakeService.GetBlogsAsync();
                return Ok(blogs.Select(c => new { c.Id, c.Title, c.Description }).ToList());
            });
        }

        /// <summary>
        /// This method shows all blogs by title from Snowflake
        /// </summary>
        [HttpGet("{title}")]
        public async Task<IActionResult> GetByTitle(string title)
        {
            return await TryExecuteAsync(async () =>
            {
                var blogs = await _snowflakeService.GetBlogsByTitleAsync(title);
                return Ok(blogs.Select(c => new { c.Id, c.Title, c.Description }).ToList());
            });
        }

        /// <summary>
        /// Test Snowflake connection
        /// </summary>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            return await TryExecuteAsync(async () =>
            {
                var isConnected = await _snowflakeService.TestConnectionAsync();
                if (isConnected)
                {
                    return Ok(new { Status = "Connected", Database = "Snowflake" });
                }

                return BadRequest(new { Status = "Failed", Error = "Could not establish connection", Database = "Snowflake" });
            });
        }

        private async Task<IActionResult> TryExecuteAsync(Func<Task<IActionResult>> operation)
        {
            // Attempt #1
            if (!_snowflakeService.IsConfigured)
            {
                return BadRequest(new
                {
                    Status = "Not Configured",
                    Error = "Snowflake is not configured. Please provide ConnectionStrings:Snowflake.",
                    Database = "Snowflake"
                });
            }

            try
            {
                return await operation();
            }
            catch
            {
                // retry
            }

            // Attempt #2 (after config reload in SnowflakeService)
            if (!_snowflakeService.IsConfigured)
            {
                return BadRequest(new
                {
                    Status = "Not Configured",
                    Error = "Snowflake is not configured. Please provide ConnectionStrings:Snowflake.",
                    Database = "Snowflake"
                });
            }

            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Failed", Error = ex.Message, Database = "Snowflake" });
            }
        }
    }
}


