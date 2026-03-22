using Domain.Common.Abstractions;
using Domain.Sessions.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;
using Infrastructure.Repositories;
using Domain.Bookings.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Application.CustomerSupport.Abstractions.Repositories;

namespace Infrastructure.Extensions.Persistence;

public static class PersistenceRegistrationExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
        });

        services.AddScoped<IContactRequestRepository, ContactRequestRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}