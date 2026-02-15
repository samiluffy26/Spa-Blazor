using spa_reservas_blazor.Client.Auth;
using spa_reservas_blazor.Shared.DTOs;

namespace spa_reservas_blazor.Services;

public class ServerAuthService : IAuthService
{
    public Task<AuthResponse> Login(LoginRequest request) => throw new NotImplementedException();
    public Task<AuthResponse> Register(RegisterRequest request) => throw new NotImplementedException();
    public Task Logout() => Task.CompletedTask;
}
