using TreasuryFlow.Application.UserBalances.Inputs;

namespace TreasuryFlow.Api.UserBalances.Requests
{
    public class GetUserBalanceByPeriodRequest
    {
        public DateOnly InitialPeriod { get; set; }
        public DateOnly FinalPeriod { get; set; }

        public GetUserBalancesByPeriodInput ToInput() =>
            new()
            {
                InitialPeriod = InitialPeriod,
                FinalPeriod = FinalPeriod
            };
    }
}
