using Dapper;
using Snowflake.Data.Client;
using System.Data;
using TestBenchWebService.Services;

namespace TestBenchWebService.Models
{
    public sealed class SnowflakeService
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationReloader _reloader;

        public SnowflakeService(IConfiguration configuration, IConfigurationReloader reloader)
        {
            _configuration = configuration;
            _reloader = reloader;
        }

        public bool IsConfigured
        {
            get
            {
                _reloader.Reload();
                return !string.IsNullOrWhiteSpace(_configuration.GetConnectionString("Snowflake"));
            }
        }

        private string? GetConnectionString()
        {
            _reloader.Reload();
            return _configuration.GetConnectionString("Snowflake");
        }

        public async Task<IEnumerable<Blog>> GetBlogsAsync()
        {
            var cs = GetConnectionString();
            if (string.IsNullOrWhiteSpace(cs))
            {
                return Array.Empty<Blog>();
            }

            using var connection = new SnowflakeDbConnection { ConnectionString = cs };
            await connection.OpenAsync();

            return await connection.QueryAsync<Blog>(
                "SELECT ID as Id, TITLE as Title, DESCRIPTION as Description FROM BLOG WHERE TITLE LIKE '%Title%'");
        }

        public async Task<IEnumerable<Blog>> GetBlogsByTitleAsync(string title)
        {
            var cs = GetConnectionString();
            if (string.IsNullOrWhiteSpace(cs))
            {
                return Array.Empty<Blog>();
            }

            using var connection = new SnowflakeDbConnection { ConnectionString = cs };
            await connection.OpenAsync();

            return await connection.QueryAsync<Blog>(
                "SELECT ID as Id, TITLE as Title, DESCRIPTION as Description FROM BLOG WHERE TITLE = @title",
                new { title });
        }

        public async Task<bool> TestConnectionAsync()
        {
            var cs = GetConnectionString();
            if (string.IsNullOrWhiteSpace(cs))
            {
                return false;
            }

            try
            {
                using var connection = new SnowflakeDbConnection { ConnectionString = cs };
                await connection.OpenAsync();
                return connection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }
    }
}


