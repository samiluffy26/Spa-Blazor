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
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                
                if (!string.IsNullOrEmpty(token))
                {
                    // VERBOSE LOGGING FOR DEBUGGING
                    Console.WriteLine($"[JwtHandler] Raw token length: {token.Length}");
                    
                    // Aggressive cleaning: Remove any non-base64url characters that might have crept in
                    // JWTs only contain a-z, A-Z, 0-9, -, _, and .
                    token = token.Trim('\"', ' ', '\t', '\r', '\n');
                    
                    if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.Substring(7).Trim();
                    }

                    // Final check: A valid JWT should have at least 2 dots
                    int dotCount = token.Count(f => f == '.');
                    
                    if (dotCount >= 2)
                    {
                        if (request.Headers.Authorization == null)
                        {
                            Console.WriteLine($"[JwtHandler] Attaching sanitized token. Start: {token.Substring(0, Math.Min(10, token.Length))}... dots: {dotCount}");
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[JwtHandler] WARNING: Token in localStorage does not look like a JWT (dots: {dotCount}). Value: {token}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JwtHandler] JS Interop Error: {ex.Message}");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
