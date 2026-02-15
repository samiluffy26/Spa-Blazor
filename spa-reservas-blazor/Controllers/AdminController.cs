using Microsoft.AspNetCore.Mvc;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Shared.Entities;
using spa_reservas_blazor.Shared.DTOs;

namespace spa_reservas_blazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISettingRepository _settingRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWebHostEnvironment _env;

    public AdminController(
        ICategoryRepository categoryRepository, 
        ISettingRepository settingRepository, 
        IBookingRepository bookingRepository,
        IServiceRepository serviceRepository,
        IUserRepository userRepository,
        IWebHostEnvironment env)
    {
        _categoryRepository = categoryRepository;
        _settingRepository = settingRepository;
        _bookingRepository = bookingRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _env = env;
    }

    [HttpGet("dashboard-stats")]
    public async Task<ActionResult<DashboardStats>> GetDashboardStats()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var firstDayOfMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        
        // 1. Reservas Hoy
        var allBookings = await _bookingRepository.GetAllAsync();
        Console.WriteLine($"[AdminStats] Total bookings in DB: {allBookings.Count}");
        
        var bookingsToday = allBookings.Count(b => {
            bool dateMatch = b.Date == today;
            bool notCancelled = b.Status != BookingStatus.Cancelled;
            if (dateMatch) Console.WriteLine($"[AdminStats] Found booking for today! ID: {b.Id}, Status: {b.Status}, Match: {dateMatch && notCancelled}");
            return dateMatch && notCancelled;
        });
        
        Console.WriteLine($"[AdminStats] Calculated BookingsToday: {bookingsToday} (Today logic: {today})");

        // 2. Ingresos Mes
        var revenueMonth = allBookings
            .Where(b => b.Date >= firstDayOfMonth && b.Status != BookingStatus.Cancelled)
            .Sum(b => b.ServicePrice);

        // 3. Servicios Activos
        var services = await _serviceRepository.GetAllAsync();
        var activeServices = services.Count;

        // 4. Clientes Nuevos Mes
        var allUsers = await _userRepository.GetAllAsync();
        var newClientsMonth = allUsers.Count(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-1) && u.Role == "Client");

        // 5. Ajustes y Capacidad
        var settings = await _settingRepository.GetSettingsAsync();

        return Ok(new DashboardStats
        {
            BookingsToday = bookingsToday,
            MaxDailyBookings = settings.MaxDailyBookings,
            OpeningTime = DateTime.Today.Add(settings.OpeningTime).ToString("HH:mm"),
            ClosingTime = DateTime.Today.Add(settings.ClosingTime).ToString("HH:mm"),
            RevenueMonth = revenueMonth,
            ActiveServices = activeServices,
            NewClientsMonth = newClientsMonth
        });
    }

    // CATEGORIES
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return Ok(await _categoryRepository.GetAllAsync());
    }

    [HttpGet("categories/{id}")]
    public async Task<ActionResult<Category>> GetCategory(string id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost("categories")]
    public async Task<ActionResult> CreateCategory(Category category)
    {
        await _categoryRepository.CreateAsync(category);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("categories/{id}")]
    public async Task<ActionResult> UpdateCategory(string id, Category category)
    {
        if (id != category.Id) return BadRequest();
        await _categoryRepository.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("categories/{id}")]
    public async Task<ActionResult> DeleteCategory(string id)
    {
        await _categoryRepository.DeleteAsync(id);
        return NoContent();
    }

    // SETTINGS
    [HttpGet("settings")]
    public async Task<ActionResult<AppSettings>> GetSettings()
    {
        return Ok(await _settingRepository.GetSettingsAsync());
    }

    [HttpPost("settings")]
    public async Task<ActionResult> UpdateSettings(AppSettings settings)
    {
        await _settingRepository.UpdateSettingsAsync(settings);
        return NoContent();
    }

    // UPLOAD
    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/uploads/{uniqueFileName}";
        return Ok(new { url });
    }
}
