using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.Fixtures;

public class TreasuryFlowDbContextFixture : IDisposable
{
    public TreasuryFlowDbContext DbContext { get; }

    public TreasuryFlowDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<TreasuryFlowDbContext>()
            .UseInMemoryDatabase($"TreasuryFlow-Test-{Guid.NewGuid()}")
            .Options;

        DbContext = new TreasuryFlowDbContext(options);
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}
