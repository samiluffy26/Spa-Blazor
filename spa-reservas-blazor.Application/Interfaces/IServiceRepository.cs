using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Application.Interfaces;

public interface IServiceRepository
{
    Task<List<Service>> GetAllAsync();
    Task<Service?> GetByIdAsync(string id);
    Task CreateAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(string id);
}
