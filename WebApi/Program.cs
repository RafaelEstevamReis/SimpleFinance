using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Simple.Finance.WebApi;
using System;
using System.IO;
using System.Reflection;

AppPaths.EnsureFolders();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(AppPaths.LogFile, rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting {App} v{Version} at {Root}", ApiInfo.Title, ApiInfo.Version, AppPaths.Root);

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(cfg =>
    {
        cfg.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = ApiInfo.Title,
            Version = "v1",
            Description = "Personal finance API backed by Simple.Finance. One SQLite database per account",
        });

        var xmlFile = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        if (File.Exists(xmlFile)) cfg.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseSwagger();
    app.UseSwaggerUI(cfg =>
    {
        cfg.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApiInfo.Title} v1");
        cfg.DocumentTitle = ApiInfo.Title;
    });

    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    app.MapControllers();

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.Information("Stopped");
    Log.CloseAndFlush();
}
