using Domain.Common.Abstractions;
using Domain.Sessions.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("FitnessBookingDb"));

        services.AddScoped<ISessionRepository, SessionRepository>();
        // services.AddScoped<IBookingRepository, BookingRepository>();
        // services.AddScoped<IMembershipRepository, MembershipRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}