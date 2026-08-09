using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Simple.Finance.WebApi;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.Auth;
using Simple.Finance.WebApi.DataConverters;
using Simple.Finance.WebApi.Filters;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

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

    builder.Services.AddSingleton<ManagementDb>();
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ManagerCache>();

    builder.Services.AddAuthentication(ApiKeyDefaults.Scheme)
                    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.Scheme, null);
    // Fail closed: everything requires a Key unless explicitly marked [AllowAnonymous]
    builder.Services.AddAuthorization(cfg => cfg.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

    builder.Services.AddControllers(cfg => cfg.Filters.Add<DomainExceptionFilter>())
                    .AddJsonOptions(cfg =>
                    {
                        cfg.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
                        cfg.JsonSerializerOptions.Converters.Add(new NullableUtcDateTimeConverter());
                        // Statuses and types travel as names, not as opaque numbers
                        cfg.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(cfg =>
    {
        cfg.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = ApiInfo.Title,
            Version = "v1",
            Description = "Personal finance API backed by Simple.Finance",
        });

        cfg.AddSecurityDefinition(ApiKeyDefaults.Scheme, new OpenApiSecurityScheme
        {
            Name = ApiKeyDefaults.HeaderName,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = $"Account Key (GUID) on the '{ApiKeyDefaults.HeaderName}' header",
        });
        cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = ApiKeyDefaults.Scheme },
            }] = [],
        });

        var xmlFile = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        if (File.Exists(xmlFile)) cfg.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowOrigins",
            policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });

    var app = builder.Build();
    app.Services.GetRequiredService<ManagementDb>().Initialize();

    app.UseSerilogRequestLogging();
    app.UseSwagger();
    app.UseSwaggerUI(cfg =>
    {
        cfg.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApiInfo.Title} v1");
        cfg.DocumentTitle = ApiInfo.Title;
    });

    app.UseCors("AllowOrigins");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous().ExcludeFromDescription();
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
