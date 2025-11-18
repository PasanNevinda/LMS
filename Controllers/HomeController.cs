using LMS.Data;
using LMS.Models;
using LMS.Models.Entities;
using LMS.ViewModels.Student_ViewModles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            SignInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if ((SignInManager.IsSignedIn(User)))
            {

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    await SignInManager.SignOutAsync();
                    return RedirectToAction("Index");
                }
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                    return RedirectToAction("AdminDashBoard", "Admin");

                if (roles.Contains("Teacher"))
                    return RedirectToAction("TeacherDashBoard", "Teacher");

                if (roles.Contains("Student"))
                    return RedirectToAction("StudentDashBoard", "Student");
            }

            return View(); 
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> BrowseCourse(int page = 1, int pageSize = 20, int? CategoryId = null, string? Search = null)
        {
            var query = _context.Courses.
                Include(c => c.Category)
                .Include(c => c.Teacher)
                .AsQueryable();

            if (CategoryId.HasValue)
                query = query.Where(c => c.CategoryId == CategoryId.Value);

            if (!string.IsNullOrEmpty(Search))
                query = query.Where(c => c.Name.Contains(Search) || c.Teacher.FullName.Contains(Search));

            query = query.Where(c => c.Status == CourseStatus.Published);
            var totalItems = await query.CountAsync();

            var vm = new CourseBrowse();
            vm.Pager = new Helpers.Pager(totalItems, page, pageSize);
            vm.Courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseCardVm()
                {
                    CourseId = c.CourseId,
                    CourseName = c.Name,
                    TeacherName = c.Teacher.FullName,
                    Price = c.Price,
                    Ratings = c.Rating,
                    CourseImage = c.CourseImage
                }).ToListAsync();

            vm.categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            vm.SearchString = Search;
            vm.SelectedCategoryId = CategoryId;

            ViewData["CurrentPage"] = "BrowseCourse";
            return View(vm);
        }
    }
}
