using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.User.Entities;

namespace TreasuryFlow.Domain.UserBalance.Entities;

public class UserBalanceEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal InputAmount { get; set; }
    public decimal OutputAmount { get; set; }
    public decimal DailyBalance => InputAmount - OutputAmount;
    public decimal TotalBalance { get; private set; }
    public DateOnly Date { get; set; }

    public UserEntity User { get; set; }

    public void ApplyDailyBalance(decimal previousTotalBalance) => //TODO: previousTotalBalance é o quanto ele tinha. Buscar por UserId e Date ordenado pelo mais recente e pegar o campo DailyBalance
        TotalBalance = previousTotalBalance + DailyBalance;

}