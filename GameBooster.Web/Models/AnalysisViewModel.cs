using GameBooster.Core.Entities;

namespace GameBooster.Web.Models
{
    public class AnalysisViewModel
    {
        public List<UserStatsModel>? UserStats { get; set; }
        public List<HighEndSystemModel>? HighEndSystems { get; set; }
        public List<GpuPerformanceModel>? GpuPerformance { get; set; }
        public List<CpuCacheModel>? CpuCache { get; set; }
        
        // SP Sonucu İçin (16 GB üstü sistemler)
        public List<SystemDetailModel>? HighRamSystems { get; set; }
    }
}