using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreasuryFlow.Domain.UserBalance.Entities;

namespace TreasuryFlow.Infrastructure.UserBalance.EntityConfigurations;

public class UserBalanceEntityConfiguration : IEntityTypeConfiguration<UserBalanceEntity>
{
    public void Configure(EntityTypeBuilder<UserBalanceEntity> builder)
    {
        builder.ToTable("UserBalances");
    }
}
