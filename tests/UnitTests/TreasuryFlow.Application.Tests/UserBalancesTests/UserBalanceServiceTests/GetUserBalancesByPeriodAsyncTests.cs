using FluentAssertions;
using NSubstitute;
using TreasuryFlow.Application.Shared.Caching.Interfaces;
using TreasuryFlow.Application.Tests.Fixtures;
using TreasuryFlow.Application.UserBalances.Inputs;
using TreasuryFlow.Application.UserBalances.Outputs;
using TreasuryFlow.Application.UserBalances.Services;
using TreasuryFlow.Domain.UserBalance.Entities;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.UserBalancesTests.UserBalanceServiceTests;

public class GetUserBalancesByPeriodAsyncTests
    : IClassFixture<TreasuryFlowDbContextFixture>
{
    readonly TreasuryFlowDbContext _context;
    readonly TreasuryFlowDbContextFixture _fixture;
    readonly ICacheService _cacheService;
    readonly UserBalanceService _service;

    public GetUserBalancesByPeriodAsyncTests(
        TreasuryFlowDbContextFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.DbContext;
        _cacheService = Substitute.For<ICacheService>();
        _service = new UserBalanceService(_context, _cacheService);
    }

    [Fact]
    public async Task GetUserBalancesByPeriodAsync_WhenCacheExists_ShouldReturnCachedValue()
    {
        // Arrange
        var input = new GetUserBalancesByPeriodInput
        {
            InitialPeriod = new DateOnly(2024, 10, 1),
            FinalPeriod = new DateOnly(2024, 10, 31)
        };

        var cachedResult = new List<GetUserBalancesByPeriodOutput>
        {
            new()
            {
                Date = new DateOnly(2024, 10, 10),
                DateTotalBalance = 999
            }
        };

        _cacheService
            .GetAsync<IEnumerable<GetUserBalancesByPeriodOutput>>(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(cachedResult);

        // Act
        var result = await _service.GetUserBalancesByPeriodAsync(
            input,
            CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cachedResult);

        await _cacheService
            .DidNotReceive()
            .SetAsync(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<GetUserBalancesByPeriodOutput>>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>());

        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetUserBalancesByPeriodAsync_WhenCacheDoesNotExist_ShouldCalculateFromDatabaseAndCache()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var input = new GetUserBalancesByPeriodInput
        {
            InitialPeriod = new DateOnly(2024, 11, 1),
            FinalPeriod = new DateOnly(2024, 11, 30)
        };

        var userBalance = new UserBalanceEntity
        {
            UserId = userId,
            Date = new DateOnly(2024, 11, 10),
            InputAmount = 200,
            OutputAmount = 50
        };

        userBalance.RecalculateTotalBalance();

        await _context.UserBalances.AddAsync(userBalance);
        await _context.SaveChangesAsync();

        _cacheService
            .GetAsync<IEnumerable<GetUserBalancesByPeriodOutput>>(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns((IEnumerable<GetUserBalancesByPeriodOutput>?)null);

        // Act
        var result = await _service.GetUserBalancesByPeriodAsync(
            input,
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);

        var balance = result.First();
        balance.Date.Should().Be(userBalance.Date);
        balance.DateTotalBalance.Should().Be(150);

        await _cacheService
            .Received(1)
            .SetAsync(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<GetUserBalancesByPeriodOutput>>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>());

        await _fixture.ResetDatabaseAsync();
    }
}
