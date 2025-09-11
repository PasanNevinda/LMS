using LMS.Data;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels;
using LMS.ViewModels.CourseDetailsVms;
using LMS.ViewModels.CourseEditVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace LMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly string SessionKey = "MultiStepForm";
        private readonly ApplicationDbContext _context;
        private readonly Repository<Course> _courses;
        private readonly ICourseService _courseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorage _fileStorage;

        public CourseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICourseService courseService, IFileStorage fileStorage)
        {
            _context = context;
            _courses = new Repository<Course>(context);
            _userManager = userManager;
            _courseService = courseService;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public IActionResult CreateStep1()
        {

            return View(new Step1ViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep1(Step1ViewModel Vm)
        {
            if (!ModelState.IsValid)
                return View(Vm);

            //var sessionModel = HttpContext.Session.Get<MultistepViewModel>(SessionKey) ?? new MultistepViewModel();
            //sessionModel.Name = Vm.Name;

            //HttpContext.Session.Set(SessionKey, sessionModel);
            var userId = _userManager.GetUserId(User);
            var course = await _courseService.CreateCourseAsync(Vm.Name, userId);
            return RedirectToAction("CreateStep2", new {id = course.CourseId});
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep2(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            var userId = _userManager.GetUserId(User);
            if (course == null || userId != course.TeacherId)
                return NotFound();

            var categories = await _courseService.GetCategoriesAsync();

            var model = new Step2ViewModel()
            {
                CourseId = course.CourseId,
                Categories = categories,
                CategoryId = course.CategoryId,
                Description = course.Description,
                ImagePath = course.CourseImage,
                Name = course.Name,
                Price = course.Price,
                Status = course.Status,
                Curriculum = course.Curriculum,
                TargetAudiance = course.TargetAudiance,
                Language = course.Language
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateStep2(Step2ViewModel Vm, string action)
        {
            var userId = _userManager.GetUserId(User);

            if(action == "save")
            {
                if(Vm.DisplayImage != null)
                {
                    var imageUrl = await _fileStorage.SaveFileAsync(Vm.DisplayImage);
                    Vm.ImagePath = imageUrl;
                }

                var saved = await _courseService.UpdateCourseDetailsAsync(Vm, userId);

                if(!saved)
                {
                    ModelState.AddModelError("", "Unable to save to draft. Please try again.");
                    Vm.Categories = await _courseService.GetCategoriesAsync();
                    return View(Vm);
                }

                return RedirectToAction("TeacherDashBoard", "Teacher");

            }


            // manual validation: all must be filled
            if (string.IsNullOrWhiteSpace(Vm.Name))
                ModelState.AddModelError("Name", "Course name is required when submitting.");
            if (string.IsNullOrWhiteSpace(Vm.Description))
                ModelState.AddModelError("Description", "Course description is required when submitting.");
            if (string.IsNullOrWhiteSpace(Vm.Curriculum))
                ModelState.AddModelError("Curriculum", "Course Curriculum is required when submitting.");
            if (string.IsNullOrWhiteSpace(Vm.TargetAudiance))
                ModelState.AddModelError("TargetAudiance", "Course TargetAudiance is required when submitting.");
            if (!Vm.CategoryId.HasValue)
                ModelState.AddModelError("CategoryId", "Please select a category.");


            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid.");
                Vm.Categories = await _courseService.GetCategoriesAsync();
                return View(Vm);
            }



            if (Vm.DisplayImage != null)
            {
                var imageUrl = await _fileStorage.SaveFileAsync(Vm.DisplayImage);
                Vm.ImagePath = imageUrl;
            }
            else
                Console.WriteLine("No image uploaded.");

            var success = await _courseService.UpdateCourseDetailsAsync(Vm, userId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update course details. Please try again.");
                return View(Vm);
            }

            if (action == "submit")
            {
                var s = await _courseService.SubmitCourseForReviewAsync(Vm.CourseId, userId);
                if (!s)
                {
                    ModelState.AddModelError("", "Unable to submit course for review. Please try again.");
                    var categories = await _courseService.GetCategoriesAsync();
                    return View(Vm);
                }
            }

            return RedirectToAction("TeacherDashBoard", "Teacher");

            //var sessionModel = HttpContext.Session.Get<MultistepViewModel>(SessionKey) ?? new MultistepViewModel();
            //sessionModel.Description = Vm.Description;
            //sessionModel.Language = Vm.Language;

            //// create course and save it to db
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // current teacher
            //var course = new Course
            //{
            //    Name = sessionModel.Name,
            //    Language = sessionModel.Language,
            //    Description = sessionModel.Description,
            //    TeacherId = userId,
            //};

            //await _courses.AddAsync(course);
            //HttpContext.Session.Remove(SessionKey);

            //return RedirectToAction("Details", new { id = course.CourseId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            var userId = _userManager.GetUserId(User);
            if (course == null || course.Status != CourseStatus.Draft)
                return NotFound();

            var deleted = await _courseService.DeleteCourseAsync(id, userId);
            if (!deleted)
            {
                return BadRequest("Unable to delete the course. Please try again.");
            }
            return RedirectToAction("TeacherDashBoard", "Teacher");
        }

        // < A controller for view pending Course Details > //

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

        [HttpGet]
        public IActionResult CreateModule(int CourseId)
        {
            var vm = new  ModuleVm{ CourseId = CourseId };
            return PartialView("_ModuleFormPartial", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModule(ModuleVm vm)
        {
            if(!ModelState.IsValid)
            {
                return PartialView("_ModuleFormPartial", vm);
            }

           var module = await _courseService.CreateModuleAsync(vm);

            return PartialView("_ModulePartial", vm);
        }
    }
}
