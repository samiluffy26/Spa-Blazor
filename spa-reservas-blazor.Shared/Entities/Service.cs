namespace spa_reservas_blazor.Shared.Entities;

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
