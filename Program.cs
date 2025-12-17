var builder = WebApplication.CreateBuilder(args);

// Allow env vars (e.g. ConnectionStrings__PostgreSQL) to override settings for plug-and-play deployments.
builder.Configuration.AddEnvironmentVariables();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment);

app.Run();
