using System.Text;
using CatalogService.Api.Middleware;
using CatalogService.Core;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Services;
using CatalogService.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Endpoint.Configurations.Extensions;
using MinimalApi.Endpoint.Extensions;

namespace CatalogService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Endpoints (CatalogItem-, CatalogBrand- og CatalogTypeEndpoints)
        builder.Services.AddEndpoints();

        // Egen database – adskilt fra monolittens CatalogDb
        builder.Services.AddDbContext<CatalogContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogServiceConnection")));

        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        var catalogSettings = builder.Configuration.Get<CatalogSettings>() ?? new CatalogSettings();
        builder.Services.AddSingleton<IUriComposer>(new UriComposer(catalogSettings));

        builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // Validerer JWT udstedt af monolitten (api/authenticate). Nøglen skal være den samme.
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]!);
        builder.Services.AddAuthentication(config =>
        {
            config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(config =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        builder.Services.AddAuthorization();

        var app = builder.Build();

        ApplyMigrations(app);

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/health", () => Results.Ok("CatalogService is running"));
        app.MapEndpoints();

        app.Run();
    }

    // Opretter databasen og catalog-tabellerne (samme skema som monolitten) uden data.
    // Prøver igen, så servicen kan starte før database-containeren er klar.
    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var catalogContext = scope.ServiceProvider.GetRequiredService<CatalogContext>();

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                catalogContext.Database.Migrate();
                return;
            }
            catch (Exception ex) when (attempt < 10)
            {
                app.Logger.LogWarning(ex, "Database not ready (attempt {Attempt}/10), retrying...", attempt);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
