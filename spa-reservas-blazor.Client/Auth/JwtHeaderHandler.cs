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
        // Don't add header for auth endpoints or if already present
        if (!request.RequestUri?.AbsolutePath.Contains("/api/auth/") ?? false)
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
