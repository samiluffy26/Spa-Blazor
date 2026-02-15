using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IServiceRepository _serviceRepository;

    public BookingService(IBookingRepository bookingRepository, IServiceRepository serviceRepository)
    {
        _bookingRepository = bookingRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        // Business logic validations
        if (booking.Date < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ArgumentException("Booking date cannot be in the past.");
        }

        if (!await _bookingRepository.IsTimeSlotAvailableAsync(booking.Date, booking.Time))
        {
            throw new InvalidOperationException("Time slot is not available.");
        }

        booking.Status = BookingStatus.Confirmed; // Auto-confirm for now
        booking.CreatedAt = DateTime.UtcNow;
        
        return await _bookingRepository.CreateAsync(booking);
    }

    public async Task<List<Service>> GetServicesAsync()
    {
        return await _serviceRepository.GetAllAsync();
    }

    public async Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status)
    {
        return await _bookingRepository.GetByStatusAsync(status);
    }

    public async Task CancelBookingAsync(string id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking != null)
        {
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);
        }
    }

    public async Task<bool> IsTimeSlotAvailableAsync(DateOnly date, TimeOnly time)
    {
        return await _bookingRepository.IsTimeSlotAvailableAsync(date, time);
    }
}
