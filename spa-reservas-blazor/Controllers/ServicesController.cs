using Microsoft.AspNetCore.Mvc;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public ServicesController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Service>>> GetServices()
    {
        return Ok(await _bookingService.GetServicesAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Service>> GetService(string id)
    {
        var services = await _bookingService.GetServicesAsync();
        var service = services.FirstOrDefault(s => s.Id == id);
        
        if (service == null)
        {
            return NotFound();
        }
        
        return Ok(service);
    }
}
