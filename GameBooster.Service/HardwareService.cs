
using System.Threading.Tasks;
using GameBooster.Core.Entities;
using GameBooster.Core.Interfaces;
using GameBooster.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace GameBooster.Service 
{
    public class HardwareService : IHardwareService
    {
        private readonly GameBoosterDbContext _context;

        public HardwareService(GameBoosterDbContext context)
        {
            _context = context;
        }

        public async Task<double> CalculateFpsAsync(UserSystem system, int gameId, string resolution, string preset)
        {
            try
            {
                // 1. OYUN PUANINI BUL
                float gameScore = 50f;
                if (gameId > 0)
                {
                    var game = await _context.Games.FindAsync(gameId);
                    if (game != null) gameScore = game.DemandScore;
                }

                // 2. DONANIM VERİLERİNİ HAZIRLA
                float cpuCores = (float)system.CPU.CoreCount;
                float cpuThreads = (float)system.CPU.ThreadCount;
                float cpuGhz = (float)system.CPU.MaxFrequency;
                float cpuCache = (float)system.CPU.Cache;
                float gpuVram = (float)system.GPU.Vram;
                float gpuCores = (float)system.GPU.CoreCount;

                // 3. AYARLARI SAYISALA ÇEVİR
                
                // Pixels
                float pixels = 2073600f; // 1080p
                if (!string.IsNullOrEmpty(resolution))
                {
                    if (resolution.Contains("720")) pixels = 921600f;
                    else if (resolution.Contains("1080")) pixels = 2073600f;
                    else if (resolution.Contains("1440") || resolution.Contains("2K")) pixels = 3686400f;
                    else if (resolution.Contains("2160") || resolution.Contains("4K")) pixels = 8294400f;
                }

                // Preset (Low=1, Ultra=4)
                float graphicPreset = 3f; // High
                switch (preset?.ToLower())
                {
                    case "low": graphicPreset = 1f; break;
                    case "medium": graphicPreset = 2f; break;
                    case "high": graphicPreset = 3f; break;
                    case "ultra": graphicPreset = 4f; break;
                    default: graphicPreset = 3f; break;
                }

                Console.WriteLine($"🔍 ONNX GİRDİLERİ (Sıralı):");
                Console.WriteLine($"1.Pixels: {pixels}, 2.Preset: {graphicPreset}, 3.CPU_Core: {cpuCores}, 4.Thread: {cpuThreads}, 5.Ghz: {cpuGhz}, 6.Cache: {cpuCache}, 7.Vram: {gpuVram}, 8.GPU_Core: {gpuCores}, 9.Score: {gameScore}");

                // 4. TENSOR OLUŞTUR
                var inputData = new float[] 
                { 
                    pixels, graphicPreset, cpuCores, cpuThreads, cpuGhz, cpuCache, gpuVram, gpuCores, gameScore 
                };

                var inputTensor = new DenseTensor<float>(inputData, new[] { 1, 9 });

                // 5. MODELİ ÇALIŞTIR
                var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "best_model.onnx");
                
                using (var session = new InferenceSession(modelPath))
                {
                    var inputName = session.InputMetadata.Keys.First();
                    var inputs = new List<NamedOnnxValue>
                    {
                        NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
                    };

                    using (var results = session.Run(inputs))
                    {
                        // --- GARANTİ YÖNTEM: TİP KONTROLÜ ---
                        var resultValue = results.First().Value;
                        double predictedFps = 0;

                        if (resultValue is Tensor<long> longTensor)
                        {
                            Console.WriteLine("✅ Model Çıktı Tipi: LONG (Int64)");
                            predictedFps = (double)longTensor[0];
                        }
                        else if (resultValue is Tensor<float> floatTensor)
                        {
                            Console.WriteLine("✅ Model Çıktı Tipi: FLOAT");
                            predictedFps = (double)floatTensor[0];
                        }
                        else if (resultValue is Tensor<int> intTensor)
                        {
                            Console.WriteLine("✅ Model Çıktı Tipi: INT (Int32)");
                            predictedFps = (double)intTensor[0];
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Bilinmeyen Çıktı Tipi: {resultValue.GetType().Name}");
                            // Son çare string'e çevirip parse etmek
                            predictedFps = 30.0; 
                        }

                        Console.WriteLine($"🚀 HESAPLANAN FPS: {predictedFps}");
                        
                        if (predictedFps < 0) predictedFps = 0;
                        return Math.Round(predictedFps, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ONNX KRİTİK HATA: {ex.Message}");
                // Hata detayını görelim
                Console.WriteLine(ex.StackTrace);
                return 0;
            }
        }

        public async Task DeleteSystemByAdminAsync(int systemId)
        {
            var system = await _context.UserSystems.FindAsync(systemId);
            if(system != null) 
            {
                _context.UserSystems.Remove(system);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteUserSystemAsync(int systemId, int userId)
        {
            var system = await _context.UserSystems.FindAsync(systemId);

            if(system != null && system.AppUserId == userId) 
            {
                _context.UserSystems.Remove(system);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CPU?> GetCpuByIdAsync(int id)
        {
            return await _context.CPUs.FindAsync(id);
        }

        public async Task<List<CPU>> GetCPUsAsync()
        {
            return await _context.CPUs.OrderBy(c => c.Name).ToListAsync(); 
        }

        public async Task<List<Game>> GetGamesAsync()
        {
            return await _context.Games.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<GPU?> GetGpuByIdAsync(int id)
        {
            return await _context.GPUs.FindAsync(id);
        }


public async Task<string> GetGpuDescriptionAsync(int gpuId)
{
    string description = "Standart";

    try
    {
        // Service katmanında Context var, bağlantıyı oradan alıyoruz
        var connection = _context.Database.GetDbConnection();
        
        // Bağlantı kapalıysa aç
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
            // SQL Fonksiyonunu çağırıyoruz
            command.CommandText = $"SELECT fn_GetGpuDescription({gpuId})";
            
            // Tek bir değer (string) döneceği için ExecuteScalar kullanıyoruz
            var result = await command.ExecuteScalarAsync();

            if (result != null)
            {
                description = result.ToString();
            }
        }
    }
    catch (Exception)
    {
        // Hata olursa varsayılan dön
        return "Bilinmiyor";
    }

    return description;
}

        public async Task<List<GPU>> GetGPUsAsync()
        {
            return await _context.GPUs.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<List<SystemDetailModel>> GetSystemReportsAsync()
        {
            // Context zaten burada inject edilmiş durumda, direkt kullanıyoruz.
            return await _context.SystemDetails.ToListAsync();
        }

        public async Task<int> GetTotalHardwareCountAsync()
        {
            var gpuCount = await _context.GPUs.CountAsync();
            var cpuCount = await _context.CPUs.CountAsync();

            return gpuCount + cpuCount;
        }

        public async Task<List<UserSystem>> GetUserSystemsAsync(int userId)
        {
            return await _context.UserSystems
            .Include(u => u.GPU)
            .Include(u => u.CPU)
            .Where(u => u.AppUserId == userId)
            .OrderByDescending(u => u.CreatedDate)
            .ToListAsync();
        }

        public async Task SaveUserSystemAsync(int userId, int gpuId, int cpuId, int ram, string systemName)
        {
            var newSystem = new UserSystem 
            {
                AppUserId = userId,
                GPUId = gpuId,
                CPUId = cpuId,
                RamAmount = ram,
                SystemName = systemName,
                CreatedDate = DateTime.Now
            };

            _context.UserSystems.Add(newSystem);
            await _context.SaveChangesAsync();
        }
        public async Task<List<UserStatsModel>> GetUserStatsAsync()
{
    return await _context.ViewUserStats.ToListAsync();
}

public async Task<List<HighEndSystemModel>> GetHighEndSystemsAsync()
{
    return await _context.ViewHighEndSystems.ToListAsync();
}

public async Task<List<GpuPerformanceModel>> GetGpuPerformanceAsync()
{
    return await _context.ViewGpuPerformance.ToListAsync();
}

public async Task<List<CpuCacheModel>> GetCpuCacheReportAsync()
{
    return await _context.ViewCpuCache.ToListAsync();
}

// SP KULLANIMI: RAM Filtreleme
public async Task<List<SystemDetailModel>> GetSystemsByMinRamAsync(int minRam)
{
    // Veritabanındaki sp_GetSystemsByMinRam prosedürünü çağırır
    return await _context.SystemDetails
        .FromSqlRaw("CALL sp_GetSystemsByMinRam({0})", minRam)
        .ToListAsync();
}

        
    }
}