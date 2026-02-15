using System.Net.Http.Json;
using spa_reservas_blazor.Shared.Entities;
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
    
    // Sync version for UI binding
    bool IsTimeSlotAvailable(DateOnly date, TimeOnly time);
    // Async version for API
    Task<bool> IsTimeSlotAvailableAsync(DateOnly date, TimeOnly time);
    
    Task<List<Service>> GetServicesAsync();
    
    event Action OnChange;
}

public class BookingService : IBookingService
{
    private readonly HttpClient _http;
    private readonly ILogger<BookingService> _logger;
    
    public Booking CurrentBooking { get; private set; } = new();
    public List<Booking> Bookings { get; private set; } = new();
    public bool IsLoading { get; private set; }

    public event Action? OnChange;

    public BookingService(HttpClient http, ILogger<BookingService> logger)
    {
        _http = http;
        _logger = logger;
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
            var response = await _http.PostAsJsonAsync("api/bookings", booking);
            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<Booking>();
                if (created != null)
                {
                    Bookings.Add(created);
                    ResetCurrentBooking();
                    return created;
                }
            }
            return booking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            throw;
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
             await Task.Delay(100);
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
    
    public async Task<bool> IsTimeSlotAvailableAsync(DateOnly date, TimeOnly time)
    {
        try 
        {
            return await _http.GetFromJsonAsync<bool>($"api/bookings/check-availability?date={date:yyyy-MM-dd}&time={time:HH:mm}");
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<List<Service>> GetServicesAsync()
    {
        try
        {
            var services = await _http.GetFromJsonAsync<List<Service>>("api/services");
            return services ?? new List<Service>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching services");
            return new List<Service>();
        }
    }
}
