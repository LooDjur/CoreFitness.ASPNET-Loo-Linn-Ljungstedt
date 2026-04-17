using Domain.Bookings.Repositories;
using Domain.Common.Abstractions;
using Domain.ContactReq.Repositories;
using Domain.Sessions;
using Domain.Users.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.Persistence;

public static class PersistenceRegistrationExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" ;
        Console.WriteLine($"DEBUG: Variabeln innehåller: '{env}'");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (env.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                var sqliteConn = configuration.GetConnectionString("SQLiteConnection");
                options.UseSqlite(sqliteConn);
                Console.WriteLine($"DATABASE: Using SQLite");
            }
            else
            {
                var sqlConn = configuration.GetConnectionString("ProductionConnection");
                options.UseSqlServer(sqlConn);
                Console.WriteLine($"DATABASE: Using SQL Server");
            }
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContactRequestRepository, ContactRequestRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}