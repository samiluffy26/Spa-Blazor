using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Application.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<List<Service>> GetServicesAsync();
    Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status);
    Task<Booking?> GetBookingByIdAsync(string id);
    Task CancelBookingAsync(string id);
    Task<bool> IsTimeSlotAvailableAsync(DateOnly date, TimeOnly time);
}
