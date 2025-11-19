using LMS.Data;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels.Admin_ViewModels;
using LMS.ViewModels.Admin_ViewModels.CourseReview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SQLitePCL;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace LMS.Controllers
{
    [Authorize(Roles ="Admin")]
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
        public async Task<IActionResult> ReviewCourse(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            var reviewVm = new CourseReviewVm()
            {
                CourseId = course.CourseId,
                Name = course.Name,
                CourseStatus = course.Status,
                Description = course.Description,
                Language = course.Language.ToString(),
                TargetAudiance = course.TargetAudiance,
                CourseImage = course.CourseImage,
                PromotionVideo = course.PromotionVideo,
                Price = course.Price,
                Category = course.Category?.Name ?? "Unknown",
                TeacherName = course.Teacher?.FullName ?? "Unknown",
                ModuleList = course.Modules.Select(m => new ModuleVm()
                {
                    Name = m.Name,
                    Items = m.ContentItems.Select(c => new ModuleItem()
                    {
                        ItemName = c.StageName,
                        Url = c.FilePath,
                        ItemType = c.Type

                    }).ToList()
                }).ToList()

            };

            return PartialView("_CourseReviewModal", reviewVm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageCourse(int page = 1, int pageSize = 5, CourseStatus? status = null, int? categoryId = null, string? search = null)
        {
            
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c =>c.Teacher)
                .AsQueryable();

            if (status != null)
                query = query.Where(c => c.Status == status);

            if (categoryId.HasValue)
                query = query.Where(c=>c.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Teacher.FullName.Contains(search));

            query = query.Where(c => c.Status != CourseStatus.Draft);
            var totalItems = await query.CountAsync();


            var vm = new CourseManageVm();
            vm.CourseTable = new CourseTableVm()
            {
                Pager = new Helpers.Pager(totalItems, page, pageSize),
            };
                vm.CourseTable.Courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseDetailVm(c.Language, c.Status)
                {
                    Id = c.CourseId,
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
                return PartialView("_CourseTable", vm.CourseTable);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> RejectCourse(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
                return NotFound();

            return PartialView("~/Views/Admin/_CourseRejectForm.cshtml", courseId);
            //return BadRequest("hhhhhhhhhhhhhhhh");
        }

        [HttpPost]
        public async Task<IActionResult> RejectCourse(int courseId, string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return Json(new { success = false, message = "Reason is required" });

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound();

            course.Status = CourseStatus.Rejected;
            course.ReviewNotes = notes;
            course.ReviewedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = true });

        }

        [HttpPost]
        public async Task<IActionResult> ChangeCourseStatus([FromBody] ChangeCourseStatusRequest request)
        {
            if (request == null)
                return BadRequest("Request body is missing.");

            if (string.IsNullOrWhiteSpace(request.status))
                return BadRequest("Status is required.");

            var validStatuses = new[] { "approve", "pending" };
            if (!validStatuses.Contains(request.status.ToLower()))
                return BadRequest("Invalid status. Use 'approve' or 'pending'.");

            var course = await _context.Courses.FindAsync(request.courseId);
            if (course == null)
                return NotFound();

            if (course.Status == CourseStatus.Published)
                return BadRequest("Cannot modify a published course.");

            course.Status = request.status.ToLower() == "approve"
                ? CourseStatus.Approved
                : CourseStatus.Pending;

            course.ReviewedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}

