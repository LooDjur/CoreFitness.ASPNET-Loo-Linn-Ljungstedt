using Domain.Common.ValueObjects.Shared;
using Domain.ContactReq.Entities;
using Domain.ContactReq.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ContactRequestConfiguration : IEntityTypeConfiguration<ContactRequestEntity>
{
    public void Configure(EntityTypeBuilder<ContactRequestEntity> builder)
    {
        builder.ToTable("ContactRequests");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => ContactRequestId.Create(value).Value)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasConversion(v => v.Value, v => FirstName.Create(v).Value)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasConversion(v => v.Value, v => LastName.Create(v).Value)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion(v => v.Value, v => Email.Create(v).Value)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasConversion(v => v.Value, v => MessageBody.Create(v).Value)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? PhoneNumber.Create(v).Value : null)
            .HasMaxLength(20);

        builder.Property(x => x.Created)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.Created);
    }
}