using Microsoft.AspNetCore.Builder;
using TreasuryFlow.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

await app.RunAsync();

