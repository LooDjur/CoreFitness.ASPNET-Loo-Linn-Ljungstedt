using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations;

internal class ContactRequestConfiguration : IEntityTypeConfiguration<ContactRequestEntity>
{
    public void Configure(EntityTypeBuilder<ContactRequestEntity> builder)
    {
        builder.ToTable("ContactRequests");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired();

        builder.Property(x => x.LastName)
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired();

        builder.Property(x => x.Phone);

        builder.Property(x => x.Message)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.HasIndex(x => x.CreatedAt);
    }
}