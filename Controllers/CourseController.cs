using System.Security.Claims;
using LMS.Data;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly string SessionKey = "MultiStepForm";
        private readonly ApplicationDbContext _context;
        private readonly Repository<Course> _courses;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
            _courses = new Repository<Course>(context);
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
        public async Task<IActionResult> CreateStep2(Step2ViewModel Vm)
        {
            if (ModelState.IsValid)
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


        
    }
}
