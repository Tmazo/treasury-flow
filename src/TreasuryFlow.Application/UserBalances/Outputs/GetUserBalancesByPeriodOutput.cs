namespace TreasuryFlow.Application.UserBalances.Outputs;

public class GetUserBalancesByPeriodOutput
{
    public IEnumerable<GetUserBalancesByPeriodDto> UserBalances { get; set; }
    public DateOnly Date { get; set; }
    public decimal DateTotalBalance { get; set; }

}

public class GetUserBalancesByPeriodDto
{
    public Guid UserId { get; set; }
    public decimal InputAmount { get; set; }
    public decimal OutputAmount { get; set; }
    public decimal TotalBalance { get; set; }
    public DateOnly Date { get; set; }
}
