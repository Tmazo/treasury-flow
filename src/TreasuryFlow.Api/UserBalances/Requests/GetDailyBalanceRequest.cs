namespace TreasuryFlow.Api.UserBalances.Requests
{
    public class GetDailyBalanceRequest
    {
        public Guid? UserId { get; set; }
        public DateOnly InitialDate { get; set; }
        public DateOnly FinalDate { get; set; }
    }
}
