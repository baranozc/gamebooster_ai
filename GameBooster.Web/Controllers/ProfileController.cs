using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GameBooster.Core.Entities;
using GameBooster.Core.Interfaces;
using GameBooster.Web.Models;

namespace GameBooster.Web.Controllers
{
    [Authorize] // Yalnızca giriş yapmış kullanıcılar girebilir
    public class ProfileController : Controller
    {
        private readonly IHardwareService _hardwareService;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(IHardwareService hardwareService, UserManager<AppUser> userManager)
        {
            _hardwareService = hardwareService;
            _userManager = userManager;
        }

        // GameBooster.Web/Controllers/ProfileController.cs

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(_userManager.GetUserId(User)!);

            // 1. Kayıtlı sistemleri getir
            var mySystems = await _hardwareService.GetUserSystemsAsync(userId);

            // 2. Dropdownları doldurmak için donanım listelerini çek
            ViewBag.Gpus = await _hardwareService.GetGPUsAsync();
            ViewBag.Cpus = await _hardwareService.GetCPUsAsync();

            // RAM seçenekleri (Bunu bir static helper veya modelden de alabiliriz, şimdilik basit tutalım)
            ViewBag.RamOptions = new List<int> { 4, 8, 12, 16, 24, 32, 48, 64, 96, 128 };

            return View(mySystems);
        }

        // YENİ METOT: Sadece Donanım Kaydetmek İçin
        [HttpPost]
        public async Task<IActionResult> AddSystem(int gpuId, int cpuId, int ramAmount, string systemName)
        {
            var userId = int.Parse(_userManager.GetUserId(User)!);

            if (string.IsNullOrEmpty(systemName)) systemName = $"Sistemim ({DateTime.Now.ToShortDateString()})";

            await _hardwareService.SaveUserSystemAsync(userId, gpuId, cpuId, ramAmount, systemName);

            TempData["SuccessMessage"] = "Yeni sistem eklendi! 🖥️";
            return RedirectToAction("Index");
        }

        // 2. Ana Sayfadan Gelen Kaydetme İsteği
        [HttpPost]
        public async Task<IActionResult> SaveSystem(HomeIndexViewModel model)
        {
            // Eğer dropdownlardan seçim yapılmamışsa kaydetme
            if (model.SelectedGpuId == 0 || model.SelectedCpuId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = int.Parse(_userManager.GetUserId(User)!);

            // Sisteme otomatik bir isim verelim (Örn: Sistemim - Tarih)
            string sysName = $"Sistemim ({DateTime.Now.ToShortDateString()})";

            await _hardwareService.SaveUserSystemAsync(
                userId,
                model.SelectedGpuId,
                model.SelectedCpuId,
                model.RamAmount,
                sysName
            );

            // Başarılı mesajı verip Profil sayfasına yönlendir
            TempData["SuccessMessage"] = "Sisteminiz başarıyla kaydedildi! 🎉";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int id)
        {
            var userId = int.Parse(_userManager.GetUserId(User)!);

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