using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Infrastructure.Data;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categories;

    public CategoryRepository(MongoDbContext context)
    {
        _categories = context.Database.GetCollection<Category>("Categories");
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categories.Find(_ => true).ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(string id)
    {
        return await _categories.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Category category)
    {
        await _categories.InsertOneAsync(category);
    }

    public async Task UpdateAsync(Category category)
    {
        await _categories.ReplaceOneAsync(c => c.Id == category.Id, category);
    }

    public async Task DeleteAsync(string id)
    {
        await _categories.DeleteOneAsync(c => c.Id == id);
    }
}
