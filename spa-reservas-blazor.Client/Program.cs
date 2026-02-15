using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register HttpClient for API calls
builder.Services.AddTransient<spa_reservas_blazor.Client.Auth.JwtHeaderHandler>();
builder.Services.AddScoped(sp => 
{
    var handler = sp.GetRequiredService<spa_reservas_blazor.Client.Auth.JwtHeaderHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
});

builder.Services.AddScoped<spa_reservas_blazor.Services.IBookingService, spa_reservas_blazor.Services.BookingService>();

// Auth Services
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, spa_reservas_blazor.Client.Auth.CustomAuthStateProvider>();
builder.Services.AddScoped<spa_reservas_blazor.Client.Auth.IAuthService, spa_reservas_blazor.Client.Auth.AuthService>();

await builder.Build().RunAsync();
