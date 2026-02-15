using System.Threading.Tasks;
using MongoDB.Driver;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Infrastructure.Data;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Infrastructure.Repositories;

public class SettingRepository : ISettingRepository
{
    private readonly IMongoCollection<AppSettings> _settings;

    public SettingRepository(MongoDbContext context)
    {
        _settings = context.Database.GetCollection<AppSettings>("Settings");
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        var settings = await _settings.Find(_ => true).FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new AppSettings();
            await _settings.InsertOneAsync(settings);
        }
        return settings;
    }

    public async Task UpdateSettingsAsync(AppSettings settings)
    {
        // Ensure ID is consistent if we treat it as singleton
        var existing = await _settings.Find(_ => true).FirstOrDefaultAsync();
        if (existing != null)
        {
            settings.Id = existing.Id;
            await _settings.ReplaceOneAsync(s => s.Id == settings.Id, settings);
        }
        else
        {
            await _settings.InsertOneAsync(settings);
        }
    }
}
