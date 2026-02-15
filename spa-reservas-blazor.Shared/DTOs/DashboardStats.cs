namespace spa_reservas_blazor.Shared.DTOs;

public class DashboardStats
{
    public int BookingsToday { get; set; }
    public int NewClientsMonth { get; set; }
    public decimal RevenueMonth { get; set; }
    public int ActiveServices { get; set; }
}
