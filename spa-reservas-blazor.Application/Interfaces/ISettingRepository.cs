using System.Threading.Tasks;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Application.Interfaces;

public interface ISettingRepository
{
    Task<AppSettings> GetSettingsAsync();
    Task UpdateSettingsAsync(AppSettings settings);
}
