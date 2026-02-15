namespace spa_reservas_blazor.Shared.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Client"; // "Admin" or "Client"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
