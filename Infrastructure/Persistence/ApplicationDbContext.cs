using Domain.Common.ValueObjects.Shared;
using Domain.Sessions.Entities;
using Domain.Sessions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<SessionEntity> Sessions { get; set; }
    // public DbSet<BookingEntity> Bookings { get; set; }
    // public DbSet<MembershipEntity> Memberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}