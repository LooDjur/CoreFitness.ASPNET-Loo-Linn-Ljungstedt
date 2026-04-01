using Domain.ContactReq.Entities;
using Domain.Sessions;
using Domain.Users.Entities;
using Infrastructure.Identit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser, AppRole, string>(options)
{
    public DbSet<UserEntity> DomainUsers { get; set; }
    public DbSet<MembershipEntity> Memberships { get; set; }
    public DbSet<ContactRequestEntity> ContactRequests { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    //public DbSet<BookingEntity> Bookings { get; set; }
    // public DbSet<MembershipEntity> Memberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}