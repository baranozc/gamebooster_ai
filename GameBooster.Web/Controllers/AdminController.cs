using GameBooster.Core.Entities;
using GameBooster.Core.Interfaces;
using GameBooster.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameBooster.Web.Controllers
{
    // Eğer normal üye girmeye çalışırsa 'Access Denied' sayfasına atılır.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHardwareService _hardwareService;

        public AdminController(UserManager<AppUser> userManager, IHardwareService hardwareService)
        {
            _userManager = userManager;
            _hardwareService = hardwareService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserCount = await _userManager.Users.CountAsync();

            ViewBag.HardwareCount = await _hardwareService.GetTotalHardwareCountAsync();

            return View();
        }

        public async Task<IActionResult> UserList() 
        {
            var members = await _userManager.GetUsersInRoleAsync("Member");
            return View(members);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id) 
        {
               var user = await _userManager.FindByIdAsync(id.ToString());
               if(user != null) 
               {
                    await _userManager.DeleteAsync(user);
                    TempData["Success"] = "Kullanıcı Başarıyla Silindi";
               }
               return RedirectToAction("UserList");
        }

        public async Task<IActionResult> EditUser(int id) 
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(AppUser model) 
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if(user == null) 
            {
                return NotFound();
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            
            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded) 
            {
                TempData["Success"] = "Kullanıcı Bilgileri Başarıyla Güncellendi.";
                return RedirectToAction("Userlist");
            }

            foreach(var error in result.Errors) 
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
          
        }

        [HttpGet]
        public async Task<IActionResult> UserSystems(int id) 
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user == null) return NotFound();

            var systems = await _hardwareService.GetUserSystemsAsync(id);

            ViewBag.UserName = user.UserName;
            ViewBag.UserId = id;

            return View(systems);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int systemId, int userId) 
        {
            await _hardwareService.DeleteSystemByAdminAsync(systemId);

            TempData["Success"] = "Sistem Başarıyla Silindi (Admin Yetkisi)";

            return RedirectToAction("UserSystems", new { id = userId });
        }

        public async Task<IActionResult> SystemReports()
        {
            var rapor = await _hardwareService.GetSystemReportsAsync();
            return View(rapor);
        }

        public async Task<IActionResult> AdvancedAnalysis()
{
    var model = new AnalysisViewModel();

    // 1. View'lerden verileri çek
    model.UserStats = await _hardwareService.GetUserStatsAsync();
    model.HighEndSystems = await _hardwareService.GetHighEndSystemsAsync();
    model.GpuPerformance = await _hardwareService.GetGpuPerformanceAsync();
    model.CpuCache = await _hardwareService.GetCpuCacheReportAsync();

    // 2. SP'yi çalıştır (16 GB üstü sistemleri getir)
    model.HighRamSystems = await _hardwareService.GetSystemsByMinRamAsync(16);

    return View(model);
}
        
    }
}