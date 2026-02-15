using MongoDB.Driver;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<Booking> Bookings => _database.GetCollection<Booking>("Bookings");
    public IMongoCollection<Service> Services => _database.GetCollection<Service>("Services");
    public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
    public IMongoCollection<AppSettings> Settings => _database.GetCollection<AppSettings>("Settings");
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
}
