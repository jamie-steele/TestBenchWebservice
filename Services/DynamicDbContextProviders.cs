using Microsoft.EntityFrameworkCore;
using TestBenchWebService.Models;

namespace TestBenchWebService.Services
{
    public interface IDynamicDbContextProvider<out TContext> where TContext : DbContext
    {
        /// <summary>
        /// Creates a DbContext using the latest configuration. Returns null if the DB isn't configured.
        /// </summary>
        TContext? Create();
    }

    public abstract class DynamicDbContextProviderBase<TContext> : IDynamicDbContextProvider<TContext>
        where TContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationReloader _reloader;
        private readonly string _connectionStringName;

        protected DynamicDbContextProviderBase(
            IConfiguration configuration,
            IConfigurationReloader reloader,
            string connectionStringName)
        {
            _configuration = configuration;
            _reloader = reloader;
            _connectionStringName = connectionStringName;
        }

        public TContext? Create()
        {
            _reloader.Reload();
            var cs = _configuration.GetConnectionString(_connectionStringName);
            if (string.IsNullOrWhiteSpace(cs))
            {
                return null;
            }

            var options = new DbContextOptionsBuilder<TContext>();
            Configure(options, cs);
            return (TContext)Activator.CreateInstance(typeof(TContext), options.Options)!;
        }

        protected abstract void Configure(DbContextOptionsBuilder<TContext> options, string connectionString);
    }

    public sealed class PostgresDbContextProvider : DynamicDbContextProviderBase<ApiDbContext>
    {
        public PostgresDbContextProvider(IConfiguration configuration, IConfigurationReloader reloader)
            : base(configuration, reloader, "PostgreSQL")
        {
        }

        protected override void Configure(DbContextOptionsBuilder<ApiDbContext> options, string connectionString)
        {
            options.UseNpgsql(connectionString);
        }
    }

    public sealed class MySqlDbContextProvider : DynamicDbContextProviderBase<MySqlDbContext>
    {
        public MySqlDbContextProvider(IConfiguration configuration, IConfigurationReloader reloader)
            : base(configuration, reloader, "MySQL")
        {
        }

        protected override void Configure(DbContextOptionsBuilder<MySqlDbContext> options, string connectionString)
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }

    public sealed class SqlServerDbContextProvider : DynamicDbContextProviderBase<SqlServerDbContext>
    {
        public SqlServerDbContextProvider(IConfiguration configuration, IConfigurationReloader reloader)
            : base(configuration, reloader, "SqlServer")
        {
        }

        protected override void Configure(DbContextOptionsBuilder<SqlServerDbContext> options, string connectionString)
        {
            options.UseSqlServer(connectionString);
        }
    }

    public sealed class OracleDbContextProvider : DynamicDbContextProviderBase<OracleDbContext>
    {
        public OracleDbContextProvider(IConfiguration configuration, IConfigurationReloader reloader)
            : base(configuration, reloader, "Oracle")
        {
        }

        protected override void Configure(DbContextOptionsBuilder<OracleDbContext> options, string connectionString)
        {
            options.UseOracle(connectionString);
        }
    }
}


