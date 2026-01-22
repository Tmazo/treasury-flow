using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TreasuryFlowDb");

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
}
