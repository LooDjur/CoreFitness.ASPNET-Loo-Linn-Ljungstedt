using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => UserId.Create(value).Value)
            .ValueGeneratedNever()
            .IsRequired();

        builder.ComplexProperty(u => u.Email, b =>
        {
            b.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });


        builder.Property(u => u.FirstName)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? FirstName.Create(v).Value : null)
            .HasColumnName("FirstName")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(u => u.LastName)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? LastName.Create(v).Value : null)
            .HasColumnName("LastName")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(u => u.Phone)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? PhoneNumber.Create(v).Value : null)
            .HasColumnName("Phone")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.HasOne(u => u.Membership)
            .WithOne()
            .HasForeignKey<MembershipEntity>(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.Role).HasConversion<string>();
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);
    }
}