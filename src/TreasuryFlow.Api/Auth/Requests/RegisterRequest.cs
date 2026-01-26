using TreasuryFlow.Application.Auth.Services.Inputs;

namespace TreasuryFlow.Api.Auth.Requests;

public class RegisterRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public RegisterInput ToInput() =>
        new(Name, Email, Password);
}
