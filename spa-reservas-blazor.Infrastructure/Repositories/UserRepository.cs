using MongoDB.Driver;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Infrastructure.Data;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _users.Find(u => u.Email == email).AnyAsync();
    }
}
