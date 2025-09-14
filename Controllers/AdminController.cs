using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult AdminDashBoard()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }
    }
}
