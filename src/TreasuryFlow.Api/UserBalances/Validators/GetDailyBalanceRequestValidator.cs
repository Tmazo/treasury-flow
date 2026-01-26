using FluentValidation;
using TreasuryFlow.Api.UserBalances.Requests;

namespace TreasuryFlow.Api.UserBalances.Validators
{
    public class GetDailyBalanceRequestValidator
        : AbstractValidator<GetDailyBalanceRequest>
    {
        public GetDailyBalanceRequestValidator()
        {
            RuleFor(x => x.InitialDate)
                .NotEmpty()
                .WithMessage("InitialDate is required.");

            RuleFor(x => x.FinalDate)
                .NotEmpty()
                .WithMessage("FinalDate is required.");

            RuleFor(x => x)
                .Must(request => request.InitialDate <= request.FinalDate)
                .WithMessage("InitialDate must be less than or equal to FinalDate.");
        }
    }
}
