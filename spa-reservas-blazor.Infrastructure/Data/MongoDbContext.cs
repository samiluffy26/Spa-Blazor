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
}
