namespace spa_reservas_blazor.Shared.DTOs;

public class DashboardStats
{
    public int BookingsToday { get; set; }
    public int UpcomingBookings { get; set; }
    public int MaxDailyBookings { get; set; }
    public string OpeningTime { get; set; } = string.Empty;
    public string ClosingTime { get; set; } = string.Empty;
    public int NewClientsMonth { get; set; }
    public decimal RevenueMonth { get; set; }
    public int ActiveServices { get; set; }
}
