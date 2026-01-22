var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TreasuryFlow_Api>("treasuryflow-api");

builder.Build().Run();
