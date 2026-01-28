using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.User.Entities;

namespace TreasuryFlow.Domain.UserBalance.Entities;

public class UserBalanceEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal InputAmount { get; set; }
    public decimal OutputAmount { get; set; }
    public decimal TotalBalance { get; set; }
    public DateOnly Date { get; set; }

    public UserEntity User { get; set; }

    public void RecalculateTotalBalance()
    {
        TotalBalance = InputAmount - OutputAmount;
    }
}