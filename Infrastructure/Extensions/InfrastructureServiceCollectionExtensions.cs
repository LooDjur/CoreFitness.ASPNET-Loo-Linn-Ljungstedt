using Application.Abstractions.Authentication;
using Application.Faq;
using Infrastructure.Extensions.Persistence;
using Infrastructure.Identit;
using Infrastructure.Identit.Data;
using Infrastructure.Identit.Services;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddPersistence(configuration);

        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 4;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/authentication/signin";
            options.Cookie.Name = ".CoreFitness.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            options.Events = new CookieAuthenticationEvents
            {
                OnSigningIn = context =>
                {
                    if (!context.Properties.IsPersistent)
                    {
                        context.CookieOptions.Expires = null;
                    }
                    return Task.CompletedTask;
                }
            };
        })
        .AddGitHub(options =>
        {
            options.ClientId = configuration["Authentication:GitHub:ClientId"] ?? "DIN_ID";
            options.ClientSecret = configuration["Authentication:GitHub:ClientSecret"] ?? "DIN_SECRET";

            options.CallbackPath = new PathString("/signin-github");

            options.Scope.Add("user:email");
        });

        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromMinutes(30);
        });

        services.AddScoped<IAuthService, IdentityAuthService>();
        services.AddScoped<IFaqService, FaqService>();

        return services;
    }

    public static async Task UseDatabaseInitialization(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            await context.Database.EnsureCreatedAsync();
            await IdentityInitializer.AddDefaultAdminAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInit");
            logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }
}
