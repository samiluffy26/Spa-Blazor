namespace spa_reservas_blazor.Models;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

public class Booking
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ServiceId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public int ServiceDuration { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Service
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; } // En minutos
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
