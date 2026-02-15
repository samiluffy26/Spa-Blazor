using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace spa_reservas_blazor.Client.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        if (!string.IsNullOrEmpty(token)) token = token.Trim('\"', ' ', '\t', '\r', '\n');

        var identity = new ClaimsIdentity();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claims = ParseClaimsFromJwt(token);
                identity = new ClaimsIdentity(claims, "jwt");
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
        if (!string.IsNullOrEmpty(token)) token = token.Trim('\"', ' ', '\t', '\r', '\n');
        var identity = new ClaimsIdentity();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claims = ParseClaimsFromJwt(token);
                identity = new ClaimsIdentity(claims, "jwt");
            }
            catch
            {
                // Relying on GetAuthenticationStateAsync or AuthService to cleanup if needed
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
            
            // Map standard "role" claim to Microsoft's ClaimTypes.Role
            if (key == "role" || key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            {
                key = ClaimTypes.Role;
            }
            else if (key == "email" || key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" || key == "email")
            {
                key = ClaimTypes.Email;
            }
            else if (key == "name" || key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
            {
                key = ClaimTypes.Name;
            }
            else if (key == "sub" || key == "unique_name" || key == "nameid" || key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
            {
                key = ClaimTypes.NameIdentifier;
            }

            claims.Add(new Claim(key, value));
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
