namespace TreasuryFlow.Application.UserBalances.Outputs;

public class UserBalanceOutput
{
    public Guid UserId { get; set; }
    public decimal InputAmount { get; set; }
    public decimal OutputAmount { get; set; }
    public decimal DailyBalance { get; set; }
    public decimal TotalBalance { get; set; }
    public DateOnly Date { get; set; }
}
