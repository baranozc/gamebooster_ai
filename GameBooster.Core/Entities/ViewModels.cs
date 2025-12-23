namespace GameBooster.Core.Entities
{
    // 1. vw_UserStats
    public class UserStatsModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    // 2. vw_HighEndSystems (Sistemi iyi olanlar)
    public class HighEndSystemModel
    {
        public string? SystemName { get; set; }
        public string? GpuName { get; set; }
        public string? CpuName { get; set; }
    }

    // 3. vw_GpuPerformance
    public class GpuPerformanceModel
    {
        public string? Name { get; set; }
        public int Vram { get; set; }
        public int CoreCount { get; set; }
    }

    // 4. vw_CpuCacheReport
    public class CpuCacheModel
    {
        public string? Name { get; set; }
        public int Cache { get; set; }
        public int ThreadCount { get; set; }
    }
}