using Microsoft.AspNetCore.Mvc;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
         _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Booking>>> GetAllBookings()
    {
        return Ok(await _bookingService.GetAllBookingsAsync());
    }

    [HttpGet("my")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<List<Booking>>> GetMyBookings()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                    ?? User.FindFirst("email")?.Value;
        
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("User email not found in token.");
        }

        return Ok(await _bookingService.GetBookingsByEmailAsync(email));
    }

    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
    {
        try 
        {
            var created = await _bookingService.CreateBookingAsync(booking);
            return CreatedAtAction(nameof(GetBooking), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(string id) 
    {
         var booking = await _bookingService.GetBookingByIdAsync(id);
         if (booking == null) return NotFound();
         return Ok(booking);
    }
    
    [HttpGet("check-availability")]
    public async Task<ActionResult<bool>> CheckAvailability([FromQuery] string date, [FromQuery] string time)
    {
        if (!DateOnly.TryParse(date, out var dateOnly) || !TimeOnly.TryParse(time, out var timeOnly))
        {
            return BadRequest("Invalid date or time format.");
        }
        
        return Ok(await _bookingService.IsTimeSlotAvailableAsync(dateOnly, timeOnly));
    }
}
