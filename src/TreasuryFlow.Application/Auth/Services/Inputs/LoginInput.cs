namespace TreasuryFlow.Application.Auth.Services.Inputs;

public record LoginInput(
    string Name,
    string Email,
    string Password);
