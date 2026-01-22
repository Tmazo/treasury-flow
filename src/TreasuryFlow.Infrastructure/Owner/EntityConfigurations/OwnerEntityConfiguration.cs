using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreasuryFlow.Domain.Owner.Entities;

namespace TreasuryFlow.Infrastructure.Owner.EntityConfigurations;

public class OwnerEntityConfiguration : IEntityTypeConfiguration<OwnerEntity>
{
    public void Configure(EntityTypeBuilder<OwnerEntity> builder)
    {
        builder.ToTable("Owners");
    }
}
