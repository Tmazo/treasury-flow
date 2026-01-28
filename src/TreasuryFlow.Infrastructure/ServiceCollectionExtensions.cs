using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TreasuryFlow.Application.Shared.Caching.Interfaces;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.Transactions.Processors.Interfaces;
using TreasuryFlow.Infrastructure.Shared.Caching.Services;
using TreasuryFlow.Infrastructure.Shared.Data;
using TreasuryFlow.Infrastructure.Transactions.Processors;

namespace TreasuryFlow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //var connectionString = configuration.GetConnectionString("TreasuryFlowDb");

        //services.AddDbContext<TreasuryFlowDbContext>(options =>
        //    options.UseSqlServer(connectionString));

        services.AddDbContext<TreasuryFlowDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("TreasuryFlowDb"),
                sql =>
                {
                    sql.EnableRetryOnFailure();
                });
        });

        services.AddScoped<ITreasuryFlowDbContext>(sp =>
            sp.GetRequiredService<TreasuryFlowDbContext>());


        return services;
    }

    public static IServiceCollection AddProcessors(this IServiceCollection services) =>
        services.AddScoped<ITransactionCreatedProcessor, TransactionCreatedProcessor>();

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<ICacheService, MemoryCacheService>();

        return services;
    }


}
