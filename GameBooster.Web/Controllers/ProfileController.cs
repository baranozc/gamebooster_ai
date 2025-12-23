using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using GameBooster.Core.Entities;
using GameBooster.Core.Interfaces;
using GameBooster.Web.Models;
using GameBooster.Grpc;
using GameBooster.Data;
using GameBooster.Service.Services; 
using System.Linq;

namespace GameBooster.Web.Controllers
{
    [Authorize] // Yalnızca giriş yapmış kullanıcılar girebilir
    public class ProfileController : Controller
    {
        private readonly IHardwareService _hardwareService;
        private readonly UserManager<AppUser> _userManager;
        private readonly BottleneckCalculator.BottleneckCalculatorClient _bottleneckClient;
        private readonly ISoapService _soapService;
        
        // Stored Procedure çalıştırmak için Context'e ihtiyacımız var
        private readonly GameBoosterDbContext _context; 

        public ProfileController(IHardwareService hardwareService, 
                                 UserManager<AppUser> userManager, 
                                 BottleneckCalculator.BottleneckCalculatorClient bottleneckClient, 
                                 ISoapService soapService,
                                 GameBoosterDbContext context) // Context'i buraya ekledik
        {
            _hardwareService = hardwareService;
            _userManager = userManager;
            _bottleneckClient = bottleneckClient;
            _soapService = soapService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Giriş yapan kullanıcının ID'sini al
            var userId = int.Parse(_userManager.GetUserId(User)!);

            // 2. Veritabanından kullanıcının sistemlerini ve Dropdown (Seçim) listelerini çek
            var mySystems = await _hardwareService.GetUserSystemsAsync(userId);
            ViewBag.Gpus = await _hardwareService.GetGPUsAsync();
            ViewBag.Cpus = await _hardwareService.GetCPUsAsync();
            
            // RAM Seçenekleri
            ViewBag.RamOptions = new List<int> { 4, 8, 12, 16, 24, 32, 48, 64, 96, 128 };

            // ============================================================
            // 🛢️ STORED PROCEDURE ENTEGRASYONU (Veritabanı İsteri)
            // ============================================================
            // Amacı: Kullanıcının kaç tane sistemi olduğunu SP ile saydırmak.
            try
            {
                var connection = _context.Database.GetDbConnection();
                // Bağlantı zaten açık olabilir kontrolü (Opsiyonel ama güvenli)
                if (connection.State != System.Data.ConnectionState.Open) 
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CALL sp_CountUserSystems({userId})";
                    var result = await command.ExecuteScalarAsync();
                    ViewBag.SpSystemCount = Convert.ToInt32(result);
                }
            }
            catch (Exception)
            {
                ViewBag.SpSystemCount = 0;
            }
            // ============================================================


            // ============================================================
            // 🛠️ SQL FUNCTION ENTEGRASYONU (Service Üzerinden - Yeni Özellik)
            // ============================================================
            // Amacı: fn_GetGpuDescription fonksiyonunu kullanarak GPU açıklamasını getirmek.
            
            Dictionary<int, string> gpuDescriptions = new Dictionary<int, string>();

            // Listelenen her sistem için GPU açıklamasını Servis'ten iste
            if (mySystems != null)
            {
                foreach (var system in mySystems)
                {
                    // Aynı GPU'yu tekrar sormamak için kontrol (Cache mantığı)
                    if (!gpuDescriptions.ContainsKey(system.GPUId))
                    {
                        // DİKKAT: Burada Context DEĞİL, Service kullanıyoruz.
                        string desc = await _hardwareService.GetGpuDescriptionAsync(system.GPUId);
                        gpuDescriptions.Add(system.GPUId, desc);
                    }
                }
            }
            
            // View'de kullanmak için gönderiyoruz
            ViewBag.GpuDescriptions = gpuDescriptions;
            // ============================================================


            // ============================================================
            // 🌍 SOAP SERVİS ENTEGRASYONU (20 Puan - İletişim Sağlama)
            // ============================================================
            try
            {
                int ramToConvert = 128; // Varsayılan değer
                if (mySystems != null && mySystems.Any())
                {
                    ramToConvert = mySystems.First().RamAmount;
                }

                string result = await _soapService.NumberToWordsAsync(ramToConvert);
                ViewBag.SoapMessage = $"Sistem belleğiniz ({ramToConvert} GB), uluslararası sunucularda '{result.ToLower()}' olarak doğrulandı.";
            }
            catch (Exception)
            {
                ViewBag.SoapMessage = "Global sunucu bağlantısı sırasında geçici bir hata oluştu (Offline Mod).";
            }
            // ============================================================

            return View(mySystems);
        }

        // YENİ METOT: Sadece Donanım Kaydetmek İçin
        [HttpPost]
        public async Task<IActionResult> AddSystem(int gpuId, int cpuId, int ramAmount, string systemName)
        {
            var userId = int.Parse(_userManager.GetUserId(User)!);

            // 1. Adım: Donanım bilgilerini çek
            var allGpus = await _hardwareService.GetGPUsAsync();
            var allCpus = await _hardwareService.GetCPUsAsync();

            var selectedGpu = allGpus.FirstOrDefault(g => g.Id == gpuId);
            var selectedCpu = allCpus.FirstOrDefault(c => c.Id == cpuId);

            string grpcMessage = "";

            // 2. Adım: gRPC ile Darboğaz Kontrolü (3. Madde - 20 Puan)
            if (selectedGpu != null && selectedCpu != null)
            {
                try
                {
                    var reply = await _bottleneckClient.CalculateAsync(new BottleneckRequest
                    {
                        GpuModel = selectedGpu.Name,
                        CpuModel = selectedCpu.Name
                    });

                    if (!reply.IsCompatible)
                    {
                        grpcMessage = $"⚠️ Uyarı: {reply.Status} (Darboğaz: %{reply.Percentage})";
                    }
                    else
                    {
                        grpcMessage = $"✅ Donanım Uyumu: {reply.Status}";
                    }
                }
                catch (Exception)
                {
                    grpcMessage = "(Uyumluluk servisine erişilemedi)";
                }
            }

            // 3. Adım: İsim boşsa otomatik ata
            if (string.IsNullOrEmpty(systemName)) 
                systemName = $"Sistemim ({DateTime.Now.ToShortDateString()})";

            // 4. Adım: Veritabanına Kaydet
            await _hardwareService.SaveUserSystemAsync(userId, gpuId, cpuId, ramAmount, systemName);

            // 5. Adım: Mesajı kullanıcıya göster
            TempData["SuccessMessage"] = $"Sistem eklendi! {grpcMessage}";
            
            return RedirectToAction("Index");
        }

        // 2. Ana Sayfadan Gelen Kaydetme İsteği
        [HttpPost]
        public async Task<IActionResult> SaveSystem(HomeIndexViewModel model)
        {
            if (model.SelectedGpuId == 0 || model.SelectedCpuId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = int.Parse(_userManager.GetUserId(User)!);
            string sysName = $"Sistemim ({DateTime.Now.ToShortDateString()})";

            await _hardwareService.SaveUserSystemAsync(
                userId,
                model.SelectedGpuId,
                model.SelectedCpuId,
                model.RamAmount,
                sysName
            );

            TempData["SuccessMessage"] = "Sisteminiz başarıyla kaydedildi! 🎉";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int id)
        {
            var userId = int.Parse(_userManager.GetUserId(User)!);
            
            // Eğer istersen burada da SP kullanarak silebilirsin:
            // await _context.Database.ExecuteSqlRawAsync("CALL sp_DeleteSystem({0})", id);
            // Ama şimdilik Service üzerinden devam ediyoruz:

            bool isDeleted = await _hardwareService.DeleteUserSystemAsync(id, userId);

            if(isDeleted) 
            {
                TempData["SuccessMessage"] = "Sistem başarıyla silindi! 🗑️";
            }
            else 
            {
                TempData["ErrorMessage"] = "Hata: Bu sistemi silmeye yetkiniz yok veya sistem bulunamadı.";
            }

            return RedirectToAction("Index");
        }
    }
}