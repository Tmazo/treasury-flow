using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TreasuryFlow.Domain.Transactions.Repositories;
using TreasuryFlow.Infrastructure.Shared.Data;
using TreasuryFlow.Infrastructure.Transactions.Repositories;

namespace TreasuryFlow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TreasuryFlowDb");

        services.AddDbContext<TreasuryFlowDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<ITransactionRepository, TransactionRepository>();
}
