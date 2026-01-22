using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureServices();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<TreasuryFlow.Infrastructure.Shared.Data.TreasuryFlowDbContext>();
await context.Database.MigrateAsync();

app.Run();
