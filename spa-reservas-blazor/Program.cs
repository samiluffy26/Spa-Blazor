using MongoDB.Driver;
using spa_reservas_blazor.Infrastructure.Data;
using spa_reservas_blazor.Infrastructure.Repositories;
using spa_reservas_blazor.Components;
// Aliases to avoid conflicts
using IAppBookingService = spa_reservas_blazor.Application.Interfaces.IBookingService;
using AppBookingService = spa_reservas_blazor.Application.Services.BookingService;
using IAppBookingRepository = spa_reservas_blazor.Application.Interfaces.IBookingRepository;
using AppBookingRepository = spa_reservas_blazor.Infrastructure.Repositories.BookingRepository;
using IAppServiceRepository = spa_reservas_blazor.Application.Interfaces.IServiceRepository;
using AppServiceRepository = spa_reservas_blazor.Infrastructure.Repositories.ServiceRepository;
using spa_reservas_blazor.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); 
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// MongoDB Integration (Local)
var mongoUrl = "mongodb://localhost:27017";
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoUrl));
builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("RelaxSpaDb"));
builder.Services.AddScoped<MongoDbContext>();

// Application Layer Services (for API)
builder.Services.AddScoped<IAppBookingRepository, AppBookingRepository>();
builder.Services.AddScoped<IAppServiceRepository, AppServiceRepository>();
builder.Services.AddScoped<IAppBookingService, AppBookingService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Client Layer Services (for SSR)
// Needs HttpClient to call its own API (Loopback)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5199/") }); // Adjust port if needed, or use NavigationManager?
// Usually better to read BaseAddress from config or NavigationManager, but in Program.cs we construct it.
// For development, localhost:5000 might work, or get Kestrel address.
// A safe bet for SSR is to register the service to call the AppService Directly?
// But ClientService has extra state logic.
// So we register ClientService, and give it an HttpClient.
builder.Services.AddScoped<spa_reservas_blazor.Services.IBookingService, spa_reservas_blazor.Services.BookingService>();


// Auth
builder.Services.AddScoped<spa_reservas_blazor.Client.Auth.IAuthService, spa_reservas_blazor.Services.ServerAuthService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[AuthDebug] Authentication Failed: {context.Exception.Message}");
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"[AuthDebug] Inner Exception: {context.Exception.InnerException.Message}");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[AuthDebug] Token Validated Successfully for user: " + context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                // Already handled in middleware but good to see what the handler sees
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var tokenCheck = builder.Configuration.GetSection("AppSettings:Token").Value;
Console.WriteLine($"[AuthDebug] JWT Token Key Length: {tokenCheck?.Length ?? -1}");

var app = builder.Build();

IdentityModelEventSource.ShowPII = true;

// Header Debugging Middleware (AT THE VERY TOP)
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        var authHeader = context.Request.Headers["Authorization"];
        Console.WriteLine($"[AuthDebug] Request: {context.Request.Method} {context.Request.Path}");
        Console.WriteLine($"[AuthDebug] Header Count: {authHeader.Count}");
        
        var headerStr = authHeader.ToString();
        if (!string.IsNullOrEmpty(headerStr))
        {
            Console.WriteLine($"[AuthDebug] Received Header: {headerStr}");
            var bytes = System.Text.Encoding.UTF8.GetBytes(headerStr);
            Console.WriteLine($"[AuthDebug] Header Bytes: {BitConverter.ToString(bytes)}");
            
            if (headerStr.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = headerStr.Substring(7).Trim();
                Console.WriteLine($"[AuthDebug] Token parsed (dots: {token.Count(f => f == '.')})");
            }
        }
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    
    // Seed Database
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        await DbInitializer.SeedAsync(context);
    }
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(spa_reservas_blazor.Client._Imports).Assembly);

app.Run();
