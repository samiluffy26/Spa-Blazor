using Microsoft.AspNetCore.Mvc;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceRepository _serviceRepository;

    public ServicesController(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Service>>> GetServices()
    {
        return Ok(await _serviceRepository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Service>> GetService(string id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if (service == null) return NotFound();
        return Ok(service);
    }

    [HttpPost]
    public async Task<ActionResult> CreateService(Service service)
    {
        await _serviceRepository.CreateAsync(service);
        return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateService(string id, Service service)
    {
        if (id != service.Id) return BadRequest();
        await _serviceRepository.UpdateAsync(service);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteService(string id)
    {
        await _serviceRepository.DeleteAsync(id);
        return NoContent();
    }
}
