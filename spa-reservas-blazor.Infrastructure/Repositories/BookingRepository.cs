using MongoDB.Driver;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Infrastructure.Data;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly MongoDbContext _context;

    public BookingRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        await _context.Bookings.InsertOneAsync(booking);
        return booking;
    }

    public async Task<Booking?> GetByIdAsync(string id)
    {
        return await _context.Bookings.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings.Find(_ => true).ToListAsync();
    }

    public async Task<List<Booking>> GetByStatusAsync(BookingStatus status)
    {
        return await _context.Bookings.Find(b => b.Status == status).ToListAsync();
    }

    public async Task<List<Booking>> GetByEmailAsync(string email)
    {
        return await _context.Bookings.Find(b => b.CustomerEmail == email).ToListAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        await _context.Bookings.ReplaceOneAsync(b => b.Id == booking.Id, booking);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Bookings.DeleteOneAsync(b => b.Id == id);
    }

    public async Task<bool> IsTimeSlotAvailableAsync(DateOnly date, TimeOnly time)
    {
        // Check if any booking exists for the same date/time that is NOT cancelled
        var count = await _context.Bookings.CountDocumentsAsync(b => 
            b.Date == date && 
            b.Time == time && 
            b.Status != BookingStatus.Cancelled);
        
        return count == 0;
    }
}
