using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace spa_reservas_blazor.Client.Auth;

public class JwtHeaderHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public JwtHeaderHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var isAuthEndpoint = request.RequestUri?.AbsolutePath.Contains("/api/auth/") ?? false;
        
        if (!isAuthEndpoint)
        {
            try 
            {
                // DEBUG LOGGING
                Console.WriteLine($"[JwtHandler] Intercepting request to: {request.RequestUri}");
                
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("[JwtHandler] Token found in localStorage, attaching to header.");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    Console.WriteLine("[JwtHandler] NO token found in localStorage.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JwtHandler] Error reading from localStorage: {ex.Message}");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
