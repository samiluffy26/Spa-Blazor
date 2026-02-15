using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<spa_reservas_blazor.Services.IBookingService, spa_reservas_blazor.Services.BookingService>();

// Auth Services
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddTransient<spa_reservas_blazor.Client.Auth.JwtHeaderHandler>();
builder.Services.AddHttpClient("RelaxSpa.API", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<spa_reservas_blazor.Client.Auth.JwtHeaderHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("RelaxSpa.API"));

builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, spa_reservas_blazor.Client.Auth.CustomAuthStateProvider>();
builder.Services.AddScoped<spa_reservas_blazor.Client.Auth.IAuthService, spa_reservas_blazor.Client.Auth.AuthService>();

await builder.Build().RunAsync();
