using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(ICourseService courseService, UserManager<ApplicationUser> userManager)
        {
            _courseService = courseService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> TeacherDashBoard()
        {
            var teacher = await _userManager.GetUserAsync(User);
            if (teacher == null)
            {
                return Redirect("~/Identity/Account/Login");
            }

            var courses = await _courseService.GetTeacherCoursesAsync(teacher.Id);

            var vms = courses.Select(c => new CourseVm
            {
                CourseId = c.CourseId,
                Title = c.Name,
                ImagePath = c.CourseImage,
                Status = c.Status,
                UpdatedAt = c.UpdatedAt,
                Progress = 10, // need to be caclculated later
                ReviewdAt = c.ReviewedAt ?? DateTime.MinValue,
            }).ToList();

            var vm = new TeacherDashboardViewModel
            {
                DraftCourses = vms.Where(c => c.Status == CourseStatus.Draft).ToList().OrderByDescending(x => x.UpdatedAt).ToList(),
                PendingCourses = vms.Where(x => x.Status == CourseStatus.Pending).ToList(),
                ApprovedCourses = vms.Where(x => x.Status == CourseStatus.Approved).ToList(),
                RejectedCourses = vms.Where(x => x.Status == CourseStatus.Rejected).ToList(),
                PublishedCourses = vms.Where(x => x.Status == CourseStatus.Published).ToList()
            };

            return View(vm);
        }
    }
}
