using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TreasuryFlow.Application.Shared.Communications.Interfaces;
using TreasuryFlow.Application.Shared.Exceptions;
using TreasuryFlow.Application.Tests.Fixtures;
using TreasuryFlow.Application.Transactions.Inputs;
using TreasuryFlow.Application.Transactions.Services;
using TreasuryFlow.Application.UserBalances.Services;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.User.Entities;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.TransactionsTests.ServicesTests.TransactionServiceTests;

public class CreateAsyncTests : IClassFixture<TreasuryFlowDbContextFixture>
{
    private readonly TreasuryFlowDbContext _context;
    readonly TreasuryFlowDbContextFixture _fixture;

    public CreateAsyncTests(
        TreasuryFlowDbContextFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.DbContext;
    }

        [Fact]
    public async Task CreateAsync_WhenUserDoesNotExist_ShouldThrowForbidden()
    {
        // Arrange
        var publisher = Substitute.For<IEventPublisher>();

        var service = new TransactionService(_context, publisher);

        var input = new CreateTransactionInput(Guid.NewGuid(), 100, ETransactionType.Input);

        // Act
        Func<Task> act = () =>
            service.CreateAsync(input, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ForbiddenAccessException>()
            .WithMessage("User is not allowed.");

        await _fixture.ResetDatabaseAsync();

    }

    [Fact]
    public async Task CreateAsync_WhenUserExists_ShouldPersistAndPublish()
    {
        // Arrange
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Email = "test@test.com",
            PasswordHash = "545ccaddc"
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var publisher = Substitute.For<IEventPublisher>();

        var service = new TransactionService(_context, publisher);

        var input = new CreateTransactionInput(user.Id, 100, ETransactionType.Input);

        // Act
        var result = await service.CreateAsync(
            input,
            CancellationToken.None);

        // Assert
        result.TransactionId.Should().NotBeEmpty();

        (await _context.Transactions.CountAsync())
            .Should().Be(1);

        await publisher.Received(1)
            .SendAsRawJsonAsync(
                Arg.Any<object>(),
                Arg.Any<CancellationToken>());

        await _fixture.ResetDatabaseAsync();
    }
}
