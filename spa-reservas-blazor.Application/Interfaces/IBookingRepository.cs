using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking?> GetByIdAsync(string id);
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetByStatusAsync(BookingStatus status);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(string id);
    Task<List<Booking>> GetByEmailAsync(string email);
    Task<bool> IsTimeSlotAvailableAsync(DateOnly date, TimeOnly time);
}
