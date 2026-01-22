using TreasuryFlow.Application;
using TreasuryFlow.Infrastructure;

namespace TreasuryFlow.Api;

public static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {

        builder.Services.AddControllers();        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddServices();
        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddRepositories();

        return builder;
    }
}
