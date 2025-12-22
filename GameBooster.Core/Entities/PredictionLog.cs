namespace GameBooster.Core.Entities
{
    public class PredictionLog : BaseEntity
    {
        public int? AppUserId { get; set; } // Giriş yapmadıysa null olabilir

        // O anki sorgu verileri
        public string GameName { get; set; } = null!;
        public string GpuModel { get; set; } = null!;
        public string CpuModel { get; set; } = null!;
        public int RamAmount { get; set; }

        // ML Modelinden gelen cevap
        public double PredictedFps { get; set; }
    }
}

