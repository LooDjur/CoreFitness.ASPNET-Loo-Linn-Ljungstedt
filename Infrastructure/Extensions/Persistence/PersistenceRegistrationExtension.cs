using Domain.Common.Abstractions;
using Domain.ContactReq.Repositories;
using Domain.Sessions;
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