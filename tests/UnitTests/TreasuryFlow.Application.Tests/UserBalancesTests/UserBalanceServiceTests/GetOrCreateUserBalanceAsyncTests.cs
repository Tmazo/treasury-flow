using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TreasuryFlow.Application.Shared.Caching.Interfaces;
using TreasuryFlow.Application.Tests.Fixtures;
using TreasuryFlow.Application.UserBalances.Services;
using TreasuryFlow.Domain.UserBalance.Entities;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.UserBalancesTests.UserBalanceServiceTests;

public class GetOrCreateUserBalanceAsyncTests : IClassFixture<TreasuryFlowDbContextFixture>
{
    readonly TreasuryFlowDbContext _context;
    readonly TreasuryFlowDbContextFixture _fixture;
    readonly UserBalanceService _service;
    readonly ICacheService _cacheService;

    public GetOrCreateUserBalanceAsyncTests(
        TreasuryFlowDbContextFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.DbContext;
        _cacheService = Substitute.For<ICacheService>();
        _service = new UserBalanceService(_context, _cacheService);
    }


    [Fact]
    public async Task GetOrCreateUserBalanceAsync_WhenBalanceExists_ShouldReturnExisting()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = new DateOnly(2024, 10, 1);

        var existingBalance = new UserBalanceEntity
        {
            UserId = userId,
            Date = date,
            InputAmount = 100,
            OutputAmount = 50
        };

        await _context.UserBalances.AddAsync(existingBalance);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetOrCreateUserBalanceAsync(userId, date);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingBalance.Id);

        (await _context.UserBalances.CountAsync())
            .Should().Be(1);

        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetOrCreateUserBalanceAsync_WhenBalanceDoesNotExist_ShouldCreateAndReturn()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = new DateOnly(2024, 10, 2);

        // Act
        var result = await _service.GetOrCreateUserBalanceAsync(userId, date);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Date.Should().Be(date);

        (await _context.UserBalances.CountAsync())
            .Should().Be(1);

        await _fixture.ResetDatabaseAsync();
    }
}
