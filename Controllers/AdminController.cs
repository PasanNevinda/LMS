using LMS.Data;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels.Admin_ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SQLitePCL;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ApplicationDbContext _context;

        public AdminController(ICourseService courseService, ApplicationDbContext context)
        {
            _courseService = courseService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AdminDashBoard()
        {
            ViewData["Title"] = "Dashboard";
            var RecentCourses = await _courseService.GetPendingCoursesAsync(3);
            var vm = new DashboardViewModel
            {
                TotalCourses = 1234,
                TotalCourseIncrease = 12,
                ActiveStudents = 24324,
                TotalStudentIncrease = 4,
                PendingReviews = 5,
                TotalRevenue = 44321,
                TotalRevenueIncrease = 1.2M,
                RecentCourses = RecentCourses.Select(c => new RecentCourcesVm()
                {
                    Name = c.Name,
                    ImgUrl = c.CourseImage,
                    Category = c.Category?.Name ?? "Uncategorized",
                    Status = "Pending",
                    Submitted = c.UpdatedAt,
                    Instructor = c.Teacher?.FullName ?? "Unknown"
                }).ToList()
                // use a viewmodel to show courses that are pending review
            };
            return View(vm);  
        }



        [HttpGet]
        public IActionResult ReviewCourse()
        {
            return PartialView("_CourseReviewModal");
        }

        [HttpGet]
        public async Task<IActionResult> ManageCourse(int page = 1, int pageSize = 10, CourseStatus? status = null, int? categoryId = null, string? search = null)
        {
            
            var query = _context.Courses
                .Include(c => c.Category)
                .AsQueryable();

            if (status != null)
                query = query.Where(c => c.Status == status);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Teacher.FullName.Contains(search));

            var totalItems = await query.CountAsync();

            var vm = new CourseManageVm();
            vm.CourseTable = new CourseTableVm()
            {
                Pager = new Helpers.Pager(totalItems, page, pageSize),
            };
                vm.CourseTable.Courses = await query
                .OrderBy(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseDetailVm(c.Language, c.Status)
                {
                    Name = c.Name,
                    TeacherEmail = c.Teacher.Email,
                    TeacherName = c.Teacher.FullName,
                    ImgUrl = c.CourseImage,
                    Category = c.Category.Name,
                    Submitted = c.CreatedAt,
                    Price = c.Price
                }).ToListAsync();

            vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            vm.AllowdStatus = new List<SelectListItem>
            {
                new SelectListItem { Text = "Pending", Value = CourseStatus.Pending.ToString() },
                new SelectListItem { Text = "Rejected", Value = CourseStatus.Rejected.ToString() },
                new SelectListItem { Text = "Approved", Value = CourseStatus.Approved.ToString() },
                new SelectListItem { Text = "Published", Value = CourseStatus.Published.ToString() },
            };

            vm.SelectedStatus = status;
            vm.SearchString = search;
            vm.SelectedCategoryId = categoryId;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_CoursesTable", vm);

            return View(vm);
        }
    }
}

