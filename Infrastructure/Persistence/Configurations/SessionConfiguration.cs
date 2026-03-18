using Domain.Common.ValueObjects.Shared;
using Domain.Sessions.Entities;
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
            .IsRequired();

        builder.ComplexProperty(s => s.Title, b =>
        {
            b.Property(t => t.Value)
                .HasColumnName("Title")
                .IsRequired();
        });

        builder.ComplexProperty(s => s.Description, b =>
        {
            b.Property(d => d.Value)
                .HasColumnName("Description")
                .IsRequired();
        });

        builder.ComplexProperty(s => s.Instructor, b =>
        {
            b.Property(i => i.Value)
                .HasColumnName("Instructor")
                .IsRequired();
        });

        builder.ComplexProperty(s => s.MaxCapacity, b =>
        {
            b.Property(c => c.Value)
                .HasColumnName("Capacity")
                .IsRequired();
        });

        // TimeSlot
        builder.ComplexProperty(s => s.Schedule, b =>
        {
            b.Property(p => p.StartTime).HasColumnName("StartTime");
            b.Property(p => p.EndTime).HasColumnName("EndTime");
        });
    }
}