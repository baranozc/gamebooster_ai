
using GameBooster.Core.Entities;

namespace GameBooster.Core.Interfaces 
{
    public interface IHardwareService 
    {
        // Donanım listelerini veya belirli bir donanımı getirmeye yarayan Interface
        Task<List<GPU>> GetGPUsAsync();
        Task<List<CPU>> GetCPUsAsync();
        Task<List<Game>> GetGamesAsync();
        Task<GPU?> GetGpuByIdAsync(int id);
        Task<CPU?> GetCpuByIdAsync(int id);
        Task<double> CalculateFpsAsync(UserSystem system, int gameId, string resolution, string preset);
        Task SaveUserSystemAsync(int userId, int gpuId, int cpuId, int ram, string systemName);
        Task<List<UserSystem>> GetUserSystemsAsync(int userId);
        Task<bool> DeleteUserSystemAsync(int systemId, int userId);
        Task<int> GetTotalHardwareCountAsync();
        Task DeleteSystemByAdminAsync(int systemId);
    }
}