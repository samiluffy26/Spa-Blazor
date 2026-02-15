using System.Net.Http.Json;
using spa_reservas_blazor.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace spa_reservas_blazor.Client.Auth;

public interface IAuthService
{
    Task<AuthResponse> Login(LoginRequest request);
    Task<AuthResponse> Register(RegisterRequest request);
    Task Logout();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _jsRuntime = jsRuntime;
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception(error);
        }

        var response = await result.Content.ReadFromJsonAsync<AuthResponse>();
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", response.Token);
        
        ((CustomAuthStateProvider)_authStateProvider).UpdateAuthenticationState(response.Token);
        
        return response;
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        if (!result.IsSuccessStatusCode)
        {
             var error = await result.Content.ReadAsStringAsync();
             throw new Exception(error);
        }

        var response = await result.Content.ReadFromJsonAsync<AuthResponse>();
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", response.Token);
        
        ((CustomAuthStateProvider)_authStateProvider).UpdateAuthenticationState(response.Token);

        return response;
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        ((CustomAuthStateProvider)_authStateProvider).UpdateAuthenticationState(null);
    }
}
