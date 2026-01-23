using Microsoft.Extensions.DependencyInjection;
using TreasuryFlow.Application.OwnerBalances.Services;
using TreasuryFlow.Application.OwnerBalances.Services.Interfaces;
using TreasuryFlow.Application.Transactions.Services;
using TreasuryFlow.Application.Transactions.Services.Interfaces;

namespace TreasuryFlow.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<IOwnerBalanceService, OwnerBalanceService>();
}
