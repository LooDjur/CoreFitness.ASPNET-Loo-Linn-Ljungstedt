using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => SessionId.Create(value).Value
            )
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(s => s.Category)
            .IsRequired();

        builder.Property(s => s.Created).IsRequired();
        builder.Property(s => s.Modified);
        builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.ComplexProperty(s => s.Title, b =>
        {
            b.Property(t => t.Value)
                .HasColumnName("Title")
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.ComplexProperty(s => s.Description, b =>
        {
            b.Property(d => d.Value)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(500);
        });

        builder.ComplexProperty(s => s.Instructor, b =>
        {
            b.Property(i => i.Value)
                .HasColumnName("Instructor")
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.ComplexProperty(s => s.MaxCapacity, b =>
        {
            b.Property(c => c.Value)
                .HasColumnName("Capacity")
                .IsRequired();
        });

        builder.ComplexProperty(s => s.Schedule, b =>
        {
            b.Property(p => p.StartTime).HasColumnName("StartTime").IsRequired();
            b.Property(p => p.EndTime).HasColumnName("EndTime").IsRequired();
        });

        builder.Property(s => s.IsDeleted)
           .HasDefaultValue(false);
    }
}