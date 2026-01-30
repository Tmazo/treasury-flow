namespace TreasuryFlow.Application.Auth.Services.Inputs;

public record LoginInput(
    string Email,
    string Password);
