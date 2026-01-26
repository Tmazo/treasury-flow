using FluentValidation;
using TreasuryFlow.Api.UserBalances.Requests;

namespace TreasuryFlow.Api.UserBalances.Validators
{
    public class GetUserBalanceByPeriodRequestValidator
        : AbstractValidator<GetUserBalanceByPeriodRequest>
    {
        public GetUserBalanceByPeriodRequestValidator()
        {
            RuleFor(x => x.InitialPeriod)
                .NotEmpty()
                .WithMessage("InitialDate is required.");

            RuleFor(x => x.FinalPeriod)
                .NotEmpty()
                .WithMessage("FinalDate is required.");

            RuleFor(x => x)
                .Must(request => request.InitialPeriod <= request.FinalPeriod)
                .WithMessage("InitialDate must be less than or equal to FinalDate.");
        }
    }
}
