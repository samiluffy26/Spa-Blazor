using MongoDB.Driver;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Infrastructure.Data;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly MongoDbContext _context;

    public ServiceRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Service>> GetAllAsync()
    {
        return await _context.Services.Find(_ => true).ToListAsync();
    }

    public async Task<Service?> GetByIdAsync(string id)
    {
        return await _context.Services.Find(s => s.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task CreateAsync(Service service)
    {
        await _context.Services.InsertOneAsync(service);
    }
}
