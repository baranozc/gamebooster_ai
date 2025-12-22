using Microsoft.AspNetCore.Mvc;

namespace GameBooster.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult GeneralError(int id)
        {
            // Hata kodunu View'a gönderelim ki ekranda yazalım
            ViewBag.ErrorCode = id;
            return View();
        }
    }
}