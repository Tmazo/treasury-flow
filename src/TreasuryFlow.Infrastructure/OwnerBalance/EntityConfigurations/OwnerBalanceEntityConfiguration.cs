using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreasuryFlow.Domain.OwnerBalance.Entities;

namespace TreasuryFlow.Infrastructure.OwnerBalance.EntityConfigurations;

public class OwnerBalanceEntityConfiguration : IEntityTypeConfiguration<OwnerBalanceEntity>
{
    public void Configure(EntityTypeBuilder<OwnerBalanceEntity> builder)
    {
        builder.ToTable("OwnerBalances");
    }
}
