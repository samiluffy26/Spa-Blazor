using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace spa_reservas_blazor.Client.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        var identity = new ClaimsIdentity();
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claims = ParseClaimsFromJwt(token);
                identity = new ClaimsIdentity(claims, "jwt");
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            catch
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        return state;
    }

    public void UpdateAuthenticationState(string? token)
    {
        var identity = new ClaimsIdentity();
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claims = ParseClaimsFromJwt(token);
                identity = new ClaimsIdentity(claims, "jwt");
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            catch
            {
                // Verify if we can remove item here without async, if not, relying on GetAuthenticationStateAsync to clean up is fine
                // or we assume token passed here is valid
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        
        var claims = new List<Claim>();
        foreach (var kvp in keyValuePairs)
        {
            var key = kvp.Key;
            var value = kvp.Value.ToString();
            
            Console.WriteLine($"[AuthDebug] Raw Claim: {key} = {value}");

            // Fix: Map standard "role" claim to Microsoft's ClaimTypes.Role
            if (key == "role" || key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            {
                key = ClaimTypes.Role;
            }
            else if (key == "email" || key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
            {
                key = ClaimTypes.Email;
            }
            else if (key == "unique_name" || key == "nameid" || key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
            {
                key = ClaimTypes.NameIdentifier;
            }

            claims.Add(new Claim(key, value));
        }
        
        foreach(var c in claims)
        {
             Console.WriteLine($"[AuthDebug] Final Claim: {c.Type} = {c.Value}");
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
