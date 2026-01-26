using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TreasuryFlow.Application.Shared.Communications.Interfaces;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.Shared.Exceptions;
using TreasuryFlow.Application.Transactions.Inputs;
using TreasuryFlow.Application.Transactions.Services;
using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.User.Entities;

namespace TreasuryFlow.Application.Tests.TransactionsTests.ServicesTests.TransactionServiceTests;

public class CreateAsyncTests
{
    [Fact]
    public async Task CreateAsync_WhenUserDoesNotExist_ShouldThrowForbiddenAccessException()
    {
        // Arrange
        var context = Substitute.For<ITreasuryFlowDbContext>();

        context.Users.Returns(
            new List<UserEntity>().AsQueryable());

        var publisher = Substitute.For<IEventPublisher>();

        var service = new TransactionService(context, publisher);

        // Act
        Func<Task> act = () =>
            service.CreateAsync(Arg.Any<CreateTransactionInput>(), CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ForbiddenAccessException>()
            .WithMessage("User is not allowed.");
    }

    [Fact]
    public async Task CreateAsync_WhenUserExists_ShouldPersistAndPublish()
    {
        // Arrange
        var userId = new Guid();

        var context = Substitute.For<ITreasuryFlowDbContext>();

        context.Users.Returns(
            new List<UserEntity>()
            { 
                new()
                { 
                    Id = userId,
                    Name = "test",
                    Email = "test@test.com",
                    PasswordHash = "d7svfvs4vf84f"
                }
            }.AsQueryable());

        context.Transactions.Returns(Substitute.For<DbSet<TransactionEntity>>());

        var publisher = Substitute.For<IEventPublisher>();

        var service = new TransactionService(context, publisher);

        var input = new CreateTransactionInput(userId, 10, ETransactionType.Input);

        // Act
        var result = await service.CreateAsync(
            input,
            CancellationToken.None);

        // Assert
        result.TransactionId.Should().NotBeEmpty();

        await context.Transactions.Received(1)
            .AddAsync(Arg.Any<TransactionEntity>(), Arg.Any<CancellationToken>());

        await context.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await publisher.Received(1)
            .SendAsRawJsonAsync(
                Arg.Any<object>(),
                Arg.Any<CancellationToken>());
    }
}
