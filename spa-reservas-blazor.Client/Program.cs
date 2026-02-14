using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<spa_reservas_blazor.Services.IBookingService, spa_reservas_blazor.Services.BookingService>();

await builder.Build().RunAsync();
