using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MembershipConfiguration : IEntityTypeConfiguration<MembershipEntity>
{
    public void Configure(EntityTypeBuilder<MembershipEntity> builder)
    {
        builder.ToTable("Memberships");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(id => id.Value, value => MemberId.Create(value).Value)
            .ValueGeneratedNever();

        builder.Property(m => m.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value).Value)
            .IsRequired();

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.ExpiryDate)
            .IsRequired();

        builder.Property(m => m.Created).IsRequired();
        builder.Property(m => m.Modified);
        builder.Property(m => m.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne<UserEntity>()
            .WithOne()
            .HasForeignKey<MembershipEntity>(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}