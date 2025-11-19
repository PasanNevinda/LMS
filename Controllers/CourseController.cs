using LMS.Data;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels;
using LMS.ViewModels.CourseDetailsVms;
using LMS.ViewModels.CourseEditVM;
using LMS.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.IO;
using System.IO;
using System.Security.AccessControl;
using System.Security.Claims;
using tusdotnet;
using tusdotnet.Models;
using tusdotnet.Stores;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
namespace LMS.Controllers
{
    [Authorize(Roles ="Teacher")]
    public class CourseController : Controller
    {
        private readonly string SessionKey = "MultiStepForm";
        private readonly ApplicationDbContext _context;
        private readonly Repository<Course> _courses;
        private readonly ICourseService _courseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorage _fileStorage;
        private readonly IViewRenderService _viewRenderService;
        private readonly IWebHostEnvironment _env;

        public CourseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICourseService courseService, IFileStorage fileStorage, IViewRenderService viewRenderService)
        {
            _context = context;
            _courses = new Repository<Course>(context);
            _userManager = userManager;
            _courseService = courseService;
            _fileStorage = fileStorage;
            _viewRenderService = viewRenderService;
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
                    TempData["ErrorMessage"] = "Unable to Save, Try again!";
                    ModelState.AddModelError("", "Unable to save to draft. Please try again.");
                    Vm.Categories = await _courseService.GetCategoriesAsync();
                    return View(Vm);
                }

                return RedirectToAction("Courses", "Teacher");

            }



            if (action == "submit")
            {

                // manual validation: all must be filled
                if (string.IsNullOrWhiteSpace(Vm.Name))
                    ModelState.AddModelError("Name", "Course name is required when submitting.");
                if (string.IsNullOrWhiteSpace(Vm.Description))
                    ModelState.AddModelError("Description", "Course description is required when submitting.");
               
                if (string.IsNullOrWhiteSpace(Vm.TargetAudiance))
                    ModelState.AddModelError("TargetAudiance", "Course TargetAudiance is required when submitting.");
                if (!Vm.CategoryId.HasValue)
                    ModelState.AddModelError("CategoryId", "Please select a category.");


                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Model state is invalid.");
                    TempData["ErrorMessage"] = "Please fill in all required fields!";
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
                    TempData["ErrorMessage"] = "Some error happened during submission, please try again";
                    ModelState.AddModelError("", "Unable to update course details. Please try again.");
                    Vm.Categories = await _courseService.GetCategoriesAsync();
                    return View(Vm);
                }
                var s = await _courseService.SubmitCourseForReviewAsync(Vm.CourseId, userId);
                if (!s)
                {
                    TempData["ErrorMessage"] = "Some error happened during submission, please try again, (Sending Email)";
                    ModelState.AddModelError("", "Unable to submit course for review. Please try again.");
                    Vm.Categories = await _courseService.GetCategoriesAsync();
                    return View(Vm);
                }
            }

            return RedirectToAction("Courses", "Teacher");

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
            return RedirectToAction("Courses", "Teacher");
        }

        // < A controller for view pending Course Details > //

        [HttpGet]
        public async Task<IActionResult> Details(int id, string? returnUrl = null)
        {
            var course = await _courses.GetByIdAsync(id, new QueryOptions<Course> { Includes = "Modules, Modules.ContentItems" });
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
                    Items = m.ContentItems
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new ModuleContentItemVm
                    {
                       ModuleId = x.ModuleId,
                       ContentItemId = x.Id,
                       DisplayName = x.StageName,
                       Description = x.Description,
                       Type = x.Type,                       //Kind = x.ContentItem switch
                       //{
                       //    DocumentContent => "Document",
                       //     VideoContent => "Video",
                       //     LinkContent => "Link",
                       //     _ => "Unknown"
                       //},
                       FilePath = x.FilePath,
                    }).ToList()
                }).ToList(),
                returnUrl = returnUrl
                
            };

            return View(vm);

        }

        [HttpGet]
        public async Task<IActionResult> CreateorEditModule(int CourseId, int ModuleId)
        {
            ModuleVm vm;
            if (ModuleId > 0)
            {
                var module = await _courseService.GetModuleByIdAsync(ModuleId);
                if (module == null)
                    return NotFound();

                vm = new ModuleVm
                {

                    CourseId = module.CourseId,
                    Id = module.ModuleId,
                    Name = module.Name,
                    Description = module.Description
                };
            }
            else
                vm = new ModuleVm { CourseId = CourseId };

            return PartialView("_ModuleFormPartial", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateorEditModule(ModuleVm vm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new {success = false, html = await _viewRenderService.RenderViewAsync(ControllerContext, "_ModuleFormPartial", vm, true) });
            }

            if (vm.Id == 0)
            {
                var module = await _courseService.CreateModuleAsync(vm);
                var mvm = new ModuleVm
                {
                    Id = module.ModuleId,
                    CourseId = module.CourseId,
                    Name = module.Name,
                    Description = module.Description,
                    Items = new List<ModuleContentItemVm>(),
                };
                //return PartialView("_ModulePartial", mvm);
                return Json(new { success = true, html = await _viewRenderService.RenderViewAsync(ControllerContext, "_ModulePartial", mvm, true), isNew = true });

            }

            var updatedModule = await _courseService.UpdateModuleAsync(vm);
            var moduleVm = new ModuleVm
            {
                Id = updatedModule.ModuleId,
                CourseId = updatedModule.CourseId,
                Name = updatedModule.Name,
                Description = updatedModule.Description,
                Items = new List<ModuleContentItemVm>(),
            };
            //return PartialView("_ModulePartial", vm);
            return Json(new { success = true, html = await _viewRenderService.RenderViewAsync(ControllerContext, "_ModulePartial", moduleVm, true), isNew = false });

        }


        [HttpGet]
        public async Task<IActionResult> AddItem(int CourseId, int ModuleId)
        {
            ViewModels.CourseDetailsVms.ContentUploadVm vm;
            if (ModuleId > 0)
            {
                var module = await _courseService.GetModuleByIdAsync(ModuleId);
                if (module == null)
                    return NotFound();

                vm = new ViewModels.CourseDetailsVms.ContentUploadVm
                {

                    CourseId = module.CourseId,
                    ModuleId = module.ModuleId,
                    Name = module.Name,
                   // Description = module.Description
                };
            }
            else
                vm = new ViewModels.CourseDetailsVms.ContentUploadVm { CourseId = CourseId };

            return PartialView("_ContentFormPartial", vm);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteItem(int Id)
        {
            Console.WriteLine(Id);
            var Content = await _context.ContentItems.FindAsync(Id);
            if (Content != null)
            {
                Console.WriteLine(Content.FilePath);
                Console.WriteLine("Successfull query");
                var dest = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Content.FilePath);
                System.IO.File.Delete(dest);
                _context.ContentItems.Remove(Content);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int Id)
        {
            Console.WriteLine(Id);
            var model = await _context.ContentItems.FindAsync(Id);
            if (model != null)
            {
                var moduleVm = new ModuleContentItemVm

        {
            ModuleId = model.ModuleId,
                    ContentItemId = model.Id,
                    DisplayName = model.StageName,
                    Description = model.Description,
                    FilePath = model.FilePath,
                    Type = model.Type,
                };
                return PartialView("_EditContentPartial", moduleVm);
            }
            else
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        public async Task<IActionResult>SaveEditContent(int ContentItemId, String Description, String DisplayName)
        {
            Console.WriteLine("Editing Content ID = "+ ContentItemId);
            Console.WriteLine("Editing Content DisplayName = " + DisplayName);
            Console.WriteLine("Editing Content Description = " + Description);
            var item = _context.ContentItems.Find(ContentItemId);

            if (item == null)
                return Json(new { success = false });

            
            item.StageName = DisplayName;
            item.Description = Description;
            

           
            _context.Entry(item).Property(x => x.StageName).IsModified = true;
            _context.Entry(item).Property(x => x.Description).IsModified = true;

            
            _context.SaveChanges();

            return Json(new { success = true, ContentItemId = ContentItemId ,DisplayName = DisplayName});

        }


        [HttpPost]
        public async Task<IActionResult> AddContent(ViewModels.CourseDetailsVms.ContentUploadVm model) {

            
            var uri = model.UploadUrl;
            var fileId = Path.GetFileName(uri);
            var filepath="";

            var tusRoot = Path.Combine(Directory.GetCurrentDirectory(), "tusfiles");
            
            var incomingPath = Path.Combine(tusRoot, fileId);
            var type = model.Type;
            Console.WriteLine(type);
            var dest = "";
            string extension = Path.GetExtension(model.Name);
            string fileName = Guid.NewGuid().ToString() + extension;
            bool allowed_type = false;
            if(type != null && type.Contains("application")){
                dest = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads","Documents", fileName);
               filepath = Path.Combine("uploads/", "Documents/", fileName);
               type = "Document";
               allowed_type = true;
            } else if (type != null && type.Contains("video")) {
                dest = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads","Videos", fileName);
                filepath = Path.Combine("uploads/", "Videos/", fileName);
                type = "Video";
                allowed_type = true;
            }

            //Renaming part
            
            if (!allowed_type)
            {
                return Ok(new { success = false, path = "/uploads/" + model.Name });
            }
            if (allowed_type)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                System.IO.File.Move(incomingPath, dest);
                
                Console.WriteLine(model.Name);
                Console.WriteLine(fileName);
                

                var record = new ContentItem
                {
                    FilePath = filepath,
                    ModuleId = model.ModuleId,
                    //Size = file.Length,
                    CreatedAt = DateTime.UtcNow,
                    Description = model.Description,
                    //TeacherId = TID,
                   StageName = model.Name,
                   Type = type
                };
                _context.ContentItems.Add(record);
            }
            await _context.SaveChangesAsync();
           // var maxId = context.YourTable.Max(x => x.Id);
            var ContentItemId = _context.ContentItems.Max(x => x.Id);
            Console.WriteLine("************"); Console.WriteLine(model.Id);
            var moduleVm = new ModuleContentItemVm
            {
                ModuleId = model.ModuleId,
                //ContentItemId = model.Id,
                DisplayName = model.Name,
                Description = model.Description,
                FilePath = filepath,
                Type = type,
            };
            //String _view = RenderViewToString(this, "_ModulePartial", model);
            //return Ok(new { success = true, html = _view });
            // return View("_ModulePartial");
            return Json(new { success = true, ModuleID = model.ModuleId, ContentItemId = ContentItemId, DisplayName = model.Name, Description = model.Description, FilePath = filepath, Type = type});


            /* var uploadedCount = 0;
             var stageNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.StagenameMap);
             string originalFileName;
             string customName;
             foreach (var file in model.Files)
             {
                 try
                 {
                     if (file.Length == 0) continue;

                     // Optional: limit file size (e.g. 50MB max)
                     const long fileLimit = 50 * 1024 * 1024;
                     if (file.Length > fileLimit)
                     {
                         ModelState.AddModelError("", $"File '{file.FileName}' exceeds the maximum allowed size.");
                         continue;
                     }

                     // Save file to local storage
                     string relativePath = await _fileStorage.SaveFileAsync(file);
                     var TID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                     originalFileName = file.FileName;
                     customName = stageNames[originalFileName];
                     // Record metadata in database
                     var record = new ContentItem
                     {
                         FilePath = relativePath,
                         //ContentType = file.ContentTyp
                         ModuleId = model.ModuleId,
                         //Size = file.Length,
                         CreatedAt = DateTime.UtcNow,
                         Description = Path.GetFileName(file.FileName),
                         //TeacherId = TID,
                         StageName = customName,
                         Type = relativePath.Split('/')[1]
                     };
                     _context.ContentItems.Add(record);
                     uploadedCount++;
                 }
                 catch (Exception ex)
                 {
                     // Log exception (omitted) and inform user
                     ModelState.AddModelError("", $"Error uploading '{file.FileName}': {ex.Message}");
                 }
             }

             await _context.SaveChangesAsync();
             ViewData["Result"] = $"{uploadedCount} file(s) uploaded successfully.";
             return Json(new { success = true, message = "Upload complete!" });*/

        }


        

    }
}
