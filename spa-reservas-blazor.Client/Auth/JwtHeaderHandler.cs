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
        // Don't modify auth endpoints
        var isAuthEndpoint = request.RequestUri?.AbsolutePath.Contains("/api/auth/") ?? false;
        
        if (!isAuthEndpoint)
        {
            try 
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                
                if (!string.IsNullOrEmpty(token))
                {
                    // Defensive cleaning: remove quotes, whitespace, and potential redundant Bearer prefix
                    token = token.Trim('\"', ' ');
                    
                    if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.Substring(7).Trim();
                    }

                    // Only attach if not already present to avoid duplicate headers which mangle the JWT
                    if (request.Headers.Authorization == null)
                    {
                        Console.WriteLine($"[JwtHandler] Attaching sanitized token (Length: {token.Length})");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    else 
                    {
                        Console.WriteLine("[JwtHandler] Authorization header already present, skipping.");
                    }
                }
                else
                {
                    Console.WriteLine("[JwtHandler] No token found in localStorage.");
                }
            }
            catch (Exception ex)
            {
                // Silently handle JS interop failures during early phase
                Console.WriteLine($"[JwtHandler] JS Interop Error: {ex.Message}");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
