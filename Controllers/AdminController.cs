using LMS.ViewModels.Admin_ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult AdminDashBoard()
        {
            ViewData["Title"] = "Dashboard";
            var vm = new DashboardViewModel
            {
                TotalCourses = 1234,
                TotalCourseIncrease = 12,
                ActiveStudents = 24324,
                TotalStudentIncrease = 4,
                PendingReviews = 5,
                TotalRevenue = 44321,
                TotalRevenueIncrease = 1.2M,
            };
            return View(vm);
        }
    }
}
