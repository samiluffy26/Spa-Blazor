using System;

namespace spa_reservas_blazor.Shared.Entities;

public class AppSettings
{
    public string Id { get; set; } = "1"; // Singleton ID
    public TimeSpan OpeningTime { get; set; } = new TimeSpan(9, 0, 0); // 09:00
    public TimeSpan ClosingTime { get; set; } = new TimeSpan(18, 0, 0); // 18:00
    public int MaxDailyBookings { get; set; } = 10;
}
