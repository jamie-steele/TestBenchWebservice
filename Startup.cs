using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TestBenchWebService.Models;
using TestBenchWebService.Services;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Self-healing: controllers will build DbContexts per-request using the latest config/env vars.
        services.AddSingleton<IConfigurationReloader, ConfigurationReloader>();
        services.AddSingleton<IBlogSeeder, BlogSeeder>();
        services.AddScoped<PostgresDbContextProvider>();
        services.AddScoped<MySqlDbContextProvider>();
        services.AddScoped<SqlServerDbContextProvider>();
        services.AddScoped<OracleDbContextProvider>();

        // Snowflake uses Dapper + Snowflake.Data (no EF DbContext).
        services.AddScoped<SnowflakeService>();

        services.AddControllers();

        // Swagger/OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Database Test Bench API",
                Version = "v1",
                Description = "Multi-Database Testing API - PostgreSQL, MySQL, SQL Server, Oracle, and Snowflake",
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                o.IncludeXmlComments(xmlPath);
            }
        });
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        // Configure URLs based on certificate availability (Docker-friendly).
        var hasCertEnvVars =
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path"));
        var hasHttpsCert = hasCertEnvVars;

        if (hasHttpsCert)
        {
            app.Urls.Clear();
            app.Urls.Add("http://0.0.0.0:8080");
            app.Urls.Add("https://0.0.0.0:443");
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
            c.RoutePrefix = string.Empty; // Swagger UI at app root
        });

        // Only use HTTPS redirection if HTTPS is configured
        if (hasHttpsCert)
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthorization();
        app.MapControllers();

        // Seed databases in background (only seeds providers that are actually registered/available).
        _ = Task.Run(async () => await DatabaseSeeder.SeedDatabasesAsync(app.Services));
    }
}


