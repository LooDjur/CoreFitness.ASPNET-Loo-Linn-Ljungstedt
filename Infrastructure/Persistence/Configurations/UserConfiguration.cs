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
            .ValueGeneratedNever();

        builder.HasOne(u => u.Membership)
            .WithOne() // Ett medlemskap tillhör en användare
            .HasForeignKey<MembershipEntity>(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ComplexProperty(u => u.Email, b =>
        {
            b.Property(e => e.Value).HasColumnName("Email").IsRequired().HasMaxLength(255);
        });

        builder.ComplexProperty(u => u.FirstName, b =>
        {
            b.Property(f => f.Value).HasColumnName("FirstName").HasMaxLength(50);
        });

        builder.ComplexProperty(u => u.LastName, b =>
        {
            b.Property(l => l.Value).HasColumnName("LastName").HasMaxLength(50);
        });

        builder.ComplexProperty(u => u.Phone, b =>
        {
            b.Property(p => p.Value).HasColumnName("Phone").HasMaxLength(20);
        });

        builder.Property(u => u.Role).HasConversion<string>();
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);
    }
}