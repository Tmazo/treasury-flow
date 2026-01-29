using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TreasuryFlow.Application.Shared.Caching.Interfaces;
using TreasuryFlow.Application.Tests.Fixtures;
using TreasuryFlow.Application.UserBalances.Services;
using TreasuryFlow.Domain.Shared.Enums;
using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.UserBalance.Entities;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.UserBalancesTests.ServicesTests.UserBalanceServiceTests;

public class UpdateUserBalanceAndTransactionAsyncTests
    : IClassFixture<TreasuryFlowDbContextFixture>
{
    readonly TreasuryFlowDbContext _context;
    readonly TreasuryFlowDbContextFixture _fixture;
    readonly UserBalanceService _service;
    readonly ICacheService _cacheService;

    public UpdateUserBalanceAndTransactionAsyncTests(
        TreasuryFlowDbContextFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.DbContext;
        _cacheService = Substitute.For<ICacheService>();
        _service = new UserBalanceService(_context, _cacheService);
    }

    [Fact]
    public async Task UpdateUserBalanceAndTransactionAsync_WhenInputTransaction_ShouldIncreaseInputAmount()
    {
        // Arrange
        var userBalance = new UserBalanceEntity
        {
            UserId = Guid.NewGuid(),
            Date = new DateOnly(2024, 10, 1),
            InputAmount = 100,
            OutputAmount = 20
        };

        var transaction = new TransactionEntity
        {
            Amount = 50,
            Type = ETransactionType.Input,
            Status = ETransactionStatus.Pending
        };

        await _context.UserBalances.AddAsync(userBalance);
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        // Act
        await _service.UpdateUserBalanceAndTransactionAsync(userBalance, transaction);

        // Assert
        var updatedBalance = await _context.UserBalances.FirstAsync();

        updatedBalance.InputAmount.Should().Be(150);
        updatedBalance.OutputAmount.Should().Be(20);
        updatedBalance.TotalBalance.Should().Be(130); // 150 - 20

        transaction.Status.Should().Be(ETransactionStatus.Processed);

        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task UpdateUserBalanceAndTransactionAsync_WhenOutputTransaction_ShouldIncreaseOutputAmount()
    {
        // Arrange
        var userBalance = new UserBalanceEntity
        {
            UserId = Guid.NewGuid(),
            Date = new DateOnly(2024, 10, 1),
            InputAmount = 200,
            OutputAmount = 30
        };

        var transaction = new TransactionEntity
        {
            Amount = 70,
            Type = ETransactionType.Output,
            Status = ETransactionStatus.Pending
        };

        await _context.UserBalances.AddAsync(userBalance);
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        // Act
        await _service.UpdateUserBalanceAndTransactionAsync(userBalance, transaction);

        // Assert
        var updatedBalance = await _context.UserBalances.FirstAsync();

        updatedBalance.InputAmount.Should().Be(200);
        updatedBalance.OutputAmount.Should().Be(100);
        updatedBalance.TotalBalance.Should().Be(100); // 200 - 100

        transaction.Status.Should().Be(ETransactionStatus.Processed);

        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task UpdateUserBalanceAndTransactionAsync_WhenTransactionTypeIsInvalid_ShouldThrow()
    {
        // Arrange
        var userBalance = new UserBalanceEntity
        {
            UserId = Guid.NewGuid(),
            Date = new DateOnly(2024, 10, 1)
        };

        var transaction = new TransactionEntity
        {
            Amount = 10,
            Type = (ETransactionType)999,
            Status = ETransactionStatus.Pending
        };

        await _context.UserBalances.AddAsync(userBalance);
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = () =>
            _service.UpdateUserBalanceAndTransactionAsync(userBalance, transaction);

        // Assert
        await act.Should()
            .ThrowAsync<NotImplementedException>()
            .WithMessage("Transaction type*");

        await _fixture.ResetDatabaseAsync();
    }
}
