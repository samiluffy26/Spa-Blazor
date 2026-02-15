using System.Collections.Generic;
using System.Threading.Tasks;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(string id);
    Task CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(string id);
}
