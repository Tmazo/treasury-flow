namespace TreasuryFlow.Application.UserBalances.Inputs;

public class GetUserBalancesByPeriodInput
{
    public DateOnly InitialPeriod { get; set; }
    public DateOnly FinalPeriod { get; set; }
}
