namespace TreasuryFlow.Application.Auth.Services.Inputs;

public record RegisterInput(
    string Name,
    string Email,
    string Password);
