using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task CreateAsync(User user);
    Task<bool> ExistsAsync(string email);
    Task<List<User>> GetAllAsync();
}
