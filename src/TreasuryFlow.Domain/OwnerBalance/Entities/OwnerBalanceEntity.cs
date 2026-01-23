using TreasuryFlow.Domain.Owner.Entities;
using TreasuryFlow.Domain.Shared.Entities;

namespace TreasuryFlow.Domain.OwnerBalance.Entities;

public class OwnerBalanceEntity : BaseEntity
{
    public Guid OwnerId { get; set; }
    public decimal InputAmount { get; set; }
    public decimal OutputAmount { get; set; }
    public decimal DailyBalance => InputAmount - OutputAmount;
    public decimal TotalBalance { get; private set; }
    public DateOnly Date { get; set; }

    public OwnerEntity Owner { get; set; }

    public void ApplyDailyBalance(decimal previousTotalBalance) => //TODO: previousTotalBalance é o quanto ele tinha. Buscar por OwnerId e Date ordenado pelo mais recente e pegar o campo DailyBalance
        TotalBalance = previousTotalBalance + DailyBalance;

}