using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.Shared.Enums;
using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.UserBalance.Entities;

namespace TreasuryFlow.Domain.User.Entities;

public class UserEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public EPermissionRole Role { get; set; }

    public IEnumerable<UserBalanceEntity>? UserBalances { get; set; }
    public IEnumerable<TransactionEntity>? Transactions { get; set; }
}
