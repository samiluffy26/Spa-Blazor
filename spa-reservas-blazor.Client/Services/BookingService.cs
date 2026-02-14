using spa_reservas_blazor.Models;
using Microsoft.Extensions.Logging;

namespace spa_reservas_blazor.Services;

public interface IBookingService
{
    Booking CurrentBooking { get; }
    List<Booking> Bookings { get; }
    bool IsLoading { get; }

    void SetService(Service service);
    void SetDate(DateOnly date);
    void SetTime(TimeOnly time);
    void SetNotes(string notes);
    void ResetCurrentBooking();

    Task<Booking> CreateBookingAsync(Booking booking);
    Task CancelBookingAsync(string bookingId);
    Task RescheduleBookingAsync(string bookingId, DateOnly newDate, TimeOnly newTime);
    
    Booking? GetBookingById(string id);
    List<Booking> GetBookingsByStatus(BookingStatus status);
    bool IsTimeSlotAvailable(DateOnly date, TimeOnly time);
    
    event Action OnChange;
}

public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    
    public Booking CurrentBooking { get; private set; } = new();
    public List<Booking> Bookings { get; private set; } = new();
    public bool IsLoading { get; private set; }

    public event Action? OnChange;

    public BookingService(ILogger<BookingService> logger)
    {
        _logger = logger;
        // Cargar datos de prueba o localStorage si fuera necesario
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void SetService(Service service)
    {
        CurrentBooking.ServiceId = service.Id;
        CurrentBooking.ServiceName = service.Name;
        CurrentBooking.ServicePrice = service.Price;
        CurrentBooking.ServiceDuration = service.Duration;
        NotifyStateChanged();
    }

    public void SetDate(DateOnly date)
    {
        CurrentBooking.Date = date;
        NotifyStateChanged();
    }

    public void SetTime(TimeOnly time)
    {
        CurrentBooking.Time = time;
        NotifyStateChanged();
    }

    public void SetNotes(string notes)
    {
        CurrentBooking.Notes = notes;
        NotifyStateChanged();
    }

    public void ResetCurrentBooking()
    {
        CurrentBooking = new Booking();
        NotifyStateChanged();
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        IsLoading = true;
        NotifyStateChanged();

        try
        {
            // Simular delay de API
            await Task.Delay(1000);
            
            booking.Id = Guid.NewGuid().ToString();
            booking.Status = BookingStatus.Confirmed;
            booking.CreatedAt = DateTime.UtcNow;
            
            Bookings.Add(booking);
            ResetCurrentBooking();
            
            return booking;
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task CancelBookingAsync(string bookingId)
    {
        IsLoading = true;
        NotifyStateChanged();

        try
        {
            await Task.Delay(500);
            var booking = Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.UpdatedAt = DateTime.UtcNow;
            }
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task RescheduleBookingAsync(string bookingId, DateOnly newDate, TimeOnly newTime)
    {
        IsLoading = true;
        NotifyStateChanged();

        try
        {
            await Task.Delay(500);
            var booking = Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.Date = newDate;
                booking.Time = newTime;
                booking.UpdatedAt = DateTime.UtcNow;
            }
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public Booking? GetBookingById(string id) => Bookings.FirstOrDefault(b => b.Id == id);

    public List<Booking> GetBookingsByStatus(BookingStatus status) => 
        Bookings.Where(b => b.Status == status).ToList();

    public bool IsTimeSlotAvailable(DateOnly date, TimeOnly time)
    {
        return !Bookings.Any(b => 
            b.Date == date && 
            b.Time == time && 
            b.Status != BookingStatus.Cancelled);
    }
}
