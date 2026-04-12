using Domain.Bookings.Entity;
using Domain.Bookings.ValueObjects;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasConversion(
                id => id.Value,
                value => BookingId.Create(value).Value)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(b => b.SessionId)
            .HasConversion(
                id => id.Value,
                value => SessionId.Create(value).Value)
            .IsRequired();

        builder.Property(b => b.MemberId)
            .HasConversion(
                id => id.Value,
                value => MemberId.Create(value).Value)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Created)
            .IsRequired();

        builder.Property(b => b.Modified);

        builder.Property(b => b.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasIndex(b => new { b.SessionId, b.MemberId });

        builder.HasOne<SessionEntity>()
            .WithMany()
            .HasForeignKey(b => b.SessionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<MembershipEntity>()
            .WithMany()
            .HasForeignKey(b => b.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}