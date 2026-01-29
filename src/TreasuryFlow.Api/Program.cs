using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Api;
using TreasuryFlow.Api.Middlewares;
using TreasuryFlow.Infrastructure.Shared.Data;

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

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<TreasuryFlowDbContext>();
await context.Database.MigrateAsync();

await app.RunAsync();
