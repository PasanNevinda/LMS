using LMS.Data;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels;
using LMS.ViewModels.CourseDetailsVms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly string SessionKey = "MultiStepForm";
        private readonly ApplicationDbContext _context;
        private readonly Repository<Course> _courses;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _courses = new Repository<Course>(context);
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateStep1()
        {

            return View(new Step1ViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStep1(Step1ViewModel Vm)
        {
            if (!ModelState.IsValid)
                return View(Vm);

            var sessionModel = HttpContext.Session.Get<MultistepViewModel>(SessionKey) ?? new MultistepViewModel();
            sessionModel.Name = Vm.Name;

            HttpContext.Session.Set(SessionKey, sessionModel);

            return RedirectToAction("CreateStep2");
        }

        [HttpGet]
        public IActionResult CreateStep2()
        {
            return View(new Step2ViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateStep2(Step2ViewModel Vm)
        {
            if (!ModelState.IsValid)
                return View(Vm);

            var sessionModel = HttpContext.Session.Get<MultistepViewModel>(SessionKey) ?? new MultistepViewModel();
            sessionModel.Description = Vm.Description;
            sessionModel.Language = Vm.Language;

            // create course and save it to db
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // current teacher
            var course = new Course
            {
                Name = sessionModel.Name,
                Language = sessionModel.Language,
                Description = sessionModel.Description,
                TeacherId = userId,
            };

            await _courses.AddAsync(course);
            HttpContext.Session.Remove(SessionKey);

            return RedirectToAction("Details", new { id = course.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courses.GetByIdAsync(id, new QueryOptions<Course> { Includes = "Modules, Modules.ModuleContentItems, Modules.ModuleContentItems.ContentItem" });
            if (course == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var isOwner = user.Id == course.TeacherId;

            var vm = new CourseDetailsVm
            {
                Id = course.CourseId,
                Name = course.Name,
                Language = course.Language,
                Description = course.Description,
                Price = course.Price,
                IsOwner = isOwner,
                Modules = course.Modules.Select(m => new ModuleVm
                {
                    Id = m.ModuleId,
                    CourseId = m.CourseId,
                    Name = m.Name,
                    Description = m.Description,
                    Items = m.ModuleContentItems
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new ModuleContentItemVm
                    {
                       Id = x.ModuleId,
                       ContentItemId = x.ContentItemId,
                       DisplayName = x.DisplayName,
                       Kind = x.ContentItem switch
                       {
                           DocumentContent => "Document",
                            VideoContent => "Video",
                            LinkContent => "Link",
                            _ => "Unknown"
                       },
                       FilePath = x.ContentItem.FilePath
                    }).ToList()
                }).ToList()
            };

            return View(vm);

        }


    }
}
