using AspNetCoreGeneratedDocument;
using LMS.Data;
using LMS.Models.Entities;
using LMS.ViewModels.Student_ViewModles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel;

namespace LMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult StudentDashBoard()
        {
            ViewData["CurrentPage"] = "StudentDashBoard";
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> BrowseCourse(int page = 1, int pageSize = 5, int? CategoryId = null, string? Search = null)
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

        public async Task<IActionResult> CourseDetails(int CourseId)
        {
            var course = await _context.Courses
                .Include(c=>c.Category)
                .Include(c => c.Teacher)
                .Include(c=> c.Modules)
                .ThenInclude(m => m.ContentItems)
                .FirstOrDefaultAsync(c => c.CourseId == CourseId);

            if (course == null)
                return NotFound();

            var vm = new CourseDetail()
            {
                CourseId = course.CourseId,
                Name = course.Name,
                Category = course.Category.Name,
                Rating = course.Rating,
                TeacherName = course.Teacher.FullName,
                LastUpdate = course.UpdatedAt,
                Description = course.Description,
                Language = course.Language.ToString(),
                Price = course.Price,
                CourseImage = course.CourseImage,
                PromotionVideo = course.PromotionVideo,
                ModuleVmList = course.Modules.Select(m => new ModuleVm()
                {
                    Name = m.Name,
                    Description= m.Description,
                    Items = m.ContentItems.Select(c => new ModuleItem()
                    {
                        ItemName = c.StageName,
                        ItemType = c.Type,
                        Description = c.Description,
                    }).ToList()
                }).ToList()

            };
            

            ViewData["CurrentPage"] = "BrowseCourse";
            return View(vm);
        }


        public IActionResult Cart()
        {
            ViewData["CurrentPage"] = "Cart";
            return View();
        }

    }
}
