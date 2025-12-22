using GameBooster.Core.Entities;

namespace GameBooster.Web.Models
{
    public class HomeIndexViewModel
    {
        // Açılır pencerelerde göstereceğimiz listeler
        public List<GPU> GpuList { get; set; } = new List<GPU>();
        public List<CPU> CpuList { get; set; } = new List<CPU>();
        public List<Game> GameList { get; set; } = new List<Game>();

        // Kullanıcının seçeceği değerler (Form submit edilince buraya dolacak)
        public int SelectedGpuId { get; set; }
        public int SelectedCpuId { get; set; }
        public int SelectedGameId { get; set; }
        public int RamAmount { get; set; }

        // --- YENİ EKLENENLER (Controller ve View Bunları Bekliyor) ---
        public string SelectedResolution { get; set; } = "1080p"; // Varsayılan değer
        public string SelectedPreset { get; set; } = "High";      // Varsayılan değer
        // -------------------------------------------------------------

        public List<int> RamOptions { get; } = new List<int> { 4, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192, 256, 512 };

        // Kullanıcının kayıtlı sistemleri
        public List<UserSystem> UserSystems { get; set; } = new List<UserSystem>();
    }
}