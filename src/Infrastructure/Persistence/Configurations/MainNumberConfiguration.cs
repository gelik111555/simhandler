using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MainNumberConfiguration : IEntityTypeConfiguration<MainNumber>
{
    public void Configure(EntityTypeBuilder<MainNumber> builder)
    {
        builder.Property(t => t.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(t => t.ICCID)
            .IsRequired()
            .HasMaxLength(25);
    }
}
