using TreasuryFlow.Application.Auth.Services.Inputs;

namespace TreasuryFlow.Application.Auth.Services.Interfaces;

public interface IAuthService
{
    Task Register(RegisterInput input);
    Task<string> Login(LoginInput input);
}
