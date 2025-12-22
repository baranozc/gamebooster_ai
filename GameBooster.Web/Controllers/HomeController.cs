using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GameBooster.Web.Models;
using GameBooster.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using GameBooster.Core.Entities;

namespace GameBooster.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHardwareService _hardwareService;
    private readonly ISystemRequirementService _systemRequiredService;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(IHardwareService hardwareService, ISystemRequirementService systemRequiredService, UserManager<AppUser> userManager)
    {
        _hardwareService = hardwareService;
        _systemRequiredService = systemRequiredService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new HomeIndexViewModel
        {
            GpuList = await _hardwareService.GetGPUsAsync(),
            CpuList = await _hardwareService.GetCPUsAsync(),
            GameList = await _hardwareService.GetGamesAsync(),
        };

        // EĞER GİRİŞ YAPMIŞSA KULLANICI SİSTEMLERİNİ ÇEK
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = _userManager.GetUserId(User);
            if (int.TryParse(userIdStr, out int userId))
            {
                model.UserSystems = await _hardwareService.GetUserSystemsAsync(userId);
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(HomeIndexViewModel model) 
    {
        // 1. VALIDASYON (Seçimler Boş mu?)
        if (model.SelectedGpuId == 0 || model.SelectedCpuId == 0 || model.SelectedGameId == 0) 
        {
            // Listeleri tekrar doldur (Hata durumunda sayfa boş gelmesin)
            model.GpuList = await _hardwareService.GetGPUsAsync();
            model.CpuList = await _hardwareService.GetCPUsAsync();
            model.GameList = await _hardwareService.GetGamesAsync();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdStr = _userManager.GetUserId(User);
                if (int.TryParse(userIdStr, out int userId))
                {
                    model.UserSystems = await _hardwareService.GetUserSystemsAsync(userId);
                }
            }

            ViewBag.Error = "Lütfen tüm seçimleri yapınız!";
            return View(model);
        }

        // 2. OYUN ADINI BUL (Gereksinim Kontrolü İçin)
        var games = await _hardwareService.GetGamesAsync();
        var selectedGame = games.FirstOrDefault(g => g.Id == model.SelectedGameId);
        string gameName = selectedGame != null ? selectedGame.Name : "Bilinmeyen Oyun";

        // 3. GEREKSİNİM KONTROLÜ
        string requirementResult = _systemRequiredService.CheckRequirement(gameName, model.RamAmount);
        TempData["Requirement_Result"] = requirementResult;

        double fps = 0;

        // Eğer sistem gereksinimleri yetmiyorsa FPS hesaplamaya gerek yok, 0 dön.
        if(requirementResult.Contains("YETERSİZ") || requirementResult.Contains("BAŞARISIZ")) 
        {
            fps = 0;
        }
        else 
        {
            // --- 🟢 YAPAY ZEKA HESAPLAMA BÖLÜMÜ ---

            // A. Seçilen parçaların detaylarını (Vram, Core vb.) servis üzerinden buluyoruz
            var allGpus = await _hardwareService.GetGPUsAsync();
            var allCpus = await _hardwareService.GetCPUsAsync();

            var realGpu = allGpus.FirstOrDefault(x => x.Id == model.SelectedGpuId);
            var realCpu = allCpus.FirstOrDefault(x => x.Id == model.SelectedCpuId);

            if (realGpu != null && realCpu != null)
            {
                // B. Hesaplama için geçici bir sistem oluşturuyoruz
                var tempSystem = new UserSystem
                {
                    GPU = realGpu,
                    CPU = realCpu,
                    RamAmount = model.RamAmount
                };

                // C. Yapay Zekayı Çağırıyoruz
                // ARTIK DİNAMİK: View'dan gelen Çözünürlük ve Ayarları kullanıyoruz!
                fps = await _hardwareService.CalculateFpsAsync(
                    tempSystem, 
                    model.SelectedGameId, 
                    model.SelectedResolution, // Örn: "4K"
                    model.SelectedPreset      // Örn: "Ultra"
                );
            }
        }

        TempData["FPS_Result"] = fps.ToString(); 

        // Post-Redirect-Get deseni (Sayfa yenilenince form tekrar gönderilmesin diye)
        return RedirectToAction("Index");
    }
}