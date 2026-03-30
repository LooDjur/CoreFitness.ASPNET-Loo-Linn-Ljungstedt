using Domain.ContactReq.Entities;
using Domain.Sessions;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<MembershipEntity> Memberships { get; set; }
    public DbSet<ContactRequestEntity> ContactRequests { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    //public DbSet<BookingEntity> Bookings { get; set; }
    // public DbSet<MembershipEntity> Memberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}