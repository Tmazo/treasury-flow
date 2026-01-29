using FluentValidation.TestHelper;
using TreasuryFlow.Api.UserBalances.Requests;
using TreasuryFlow.Api.UserBalances.Validators;

namespace TreasuryFlow.Api.Tests.UserBalancesTests.ValidatorsTests;

public class GetUserBalanceByPeriodRequestValidatorTests
{
    readonly GetUserBalanceByPeriodRequestValidator _validator;

    public GetUserBalanceByPeriodRequestValidatorTests()
    {
        _validator = new GetUserBalanceByPeriodRequestValidator();
    }

    [Fact]
    public void Should_have_error_when_initial_period_is_empty()
    {
        // Arrange
        var request = new GetUserBalanceByPeriodRequest
        {
            InitialPeriod = default,
            FinalPeriod = new DateOnly(2024, 10, 10)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InitialPeriod)
              .WithErrorMessage("InitialDate is required.");
    }

    [Fact]
    public void Should_have_error_when_final_period_is_empty()
    {
        // Arrange
        var request = new GetUserBalanceByPeriodRequest
        {
            InitialPeriod = new DateOnly(2024, 10, 1),
            FinalPeriod = default
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FinalPeriod)
              .WithErrorMessage("FinalDate is required.");
    }

    [Fact]
    public void Should_have_error_when_initial_period_is_greater_than_final_period()
    {
        // Arrange
        var request = new GetUserBalanceByPeriodRequest
        {
            InitialPeriod = new DateOnly(2024, 10, 10),
            FinalPeriod = new DateOnly(2024, 10, 1)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("InitialDate must be less than or equal to FinalDate.");
    }

    [Fact]
    public void Should_not_have_error_when_period_is_valid()
    {
        // Arrange
        var request = new GetUserBalanceByPeriodRequest
        {
            InitialPeriod = new DateOnly(2024, 10, 1),
            FinalPeriod = new DateOnly(2024, 10, 31)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}