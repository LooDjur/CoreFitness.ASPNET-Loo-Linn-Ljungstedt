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

        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (env == "Development")
            {
                options.UseSqlite("Data Source=CoreFitness.db");
            }
            else
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
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