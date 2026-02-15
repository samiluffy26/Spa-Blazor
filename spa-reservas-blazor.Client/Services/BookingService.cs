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
    bool IsTimeSlotAvailable(DateOnly date, TimeOnly time);
    List<Service> GetServices();
    
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

    public List<Service> GetServices()
    {
        return new List<Service>
        {
            new Service { Id = "1", Name = "Masaje Relajante", Category = "Masajes", Price = 60, Duration = 60, Description = "Masaje suave para aliviar estrés con aceites esenciales.", ImageUrl = "https://images.unsplash.com/photo-1519823551278-64ac92734fb1?q=80&w=800&auto=format&fit=crop" },
            new Service { Id = "2", Name = "Masaje de Tejido Profundo", Category = "Masajes", Price = 75, Duration = 60, Description = "Terapia intensa para aliviar tensión muscular crónica.", ImageUrl = "https://images.unsplash.com/photo-1544161515-4ab6ce6db874?q=80&w=800&auto=format&fit=crop" },
            new Service { Id = "3", Name = "Piedras Calientes", Category = "Masajes", Price = 85, Duration = 75, Description = "El calor de las piedras volcánicas derrite la tensión.", ImageUrl = "https://images.unsplash.com/photo-1600334089648-b0d9d3028eb2?q=80&w=800&auto=format&fit=crop" },
            
            new Service { Id = "4", Name = "Facial Rejuvenecedor", Category = "Faciales", Price = 80, Duration = 50, Description = "Limpieza profunda e hidratación para una piel radiante.", ImageUrl = "https://images.unsplash.com/photo-1570172619644-dfd03ed5d881?q=80&w=800&auto=format&fit=crop" },
            new Service { Id = "5", Name = "Facial Anti-Edad", Category = "Faciales", Price = 95, Duration = 60, Description = "Tratamiento intensivo con colágeno y vitamina C.", ImageUrl = "https://images.unsplash.com/photo-1512290923902-8a9281bf7719?q=80&w=800&auto=format&fit=crop" },
            
            new Service { Id = "6", Name = "Exfoliación Corporal", Category = "Corporales", Price = 55, Duration = 45, Description = "Renueva tu piel con sales marinas y aceites hidratantes.", ImageUrl = "https://images.unsplash.com/photo-1532434777555-9847137d1a43?q=80&w=800&auto=format&fit=crop" },
            new Service { Id = "7", Name = "Envoltura de Algas", Category = "Corporales", Price = 70, Duration = 60, Description = "Detoxifica y remineraliza tu cuerpo.", ImageUrl = "https://images.unsplash.com/photo-1515377905703-c4788e51af15?q=80&w=800&auto=format&fit=crop" },
            
            new Service { Id = "8", Name = "Circuito Hidroterapia", Category = "Spa", Price = 45, Duration = 90, Description = "Acceso a piscinas, sauna y jacuzzi por 90 minutos.", ImageUrl = "https://images.unsplash.com/photo-1563853632009-82672728271d?q=80&w=800&auto=format&fit=crop" },
            new Service { Id = "9", Name = "Ritual Parejas", Category = "Spa", Price = 150, Duration = 90, Description = "Experiencia compartida con masajes y jacuzzi privado.", ImageUrl = "https://images.unsplash.com/photo-1515377905703-c4788e51af15?q=80&w=800&auto=format&fit=crop" }
        };
    }
}
