using FluentValidation.TestHelper;
using TreasuryFlow.Api.Transactions.Requests;
using TreasuryFlow.Api.Transactions.Validators;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Api.Tests.Transactions.Validators;

public class CreateTransactionRequestValidatorTests
{
    readonly CreateTransactionRequestValidator _validator;

    public CreateTransactionRequestValidatorTests()
    {
        _validator = new CreateTransactionRequestValidator();
    }

    [Fact]
    public void Should_have_error_when_amount_is_zero_or_negative()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Amount = 0,
            TransactionType = ETransactionType.Input
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
              .WithErrorMessage("Amount must be greater than zero.");
    }

    [Fact]
    public void Should_have_error_when_transaction_type_is_invalid()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Amount = 100,
            TransactionType = (ETransactionType)999
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TransactionType)
              .WithErrorMessage("Type must be a valid transaction type.");
    }

    [Fact]
    public void Should_not_have_error_when_request_is_valid()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Amount = 150,
            TransactionType = ETransactionType.Input
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
