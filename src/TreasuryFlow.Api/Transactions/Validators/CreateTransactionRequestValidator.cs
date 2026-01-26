using FluentValidation;
using TreasuryFlow.Api.Transactions.Requests;

namespace TreasuryFlow.Api.Transactions.Validators
{
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.TransactionType)
                .IsInEnum().WithMessage("Type must be a valid transaction type.");
        }
    }
}
