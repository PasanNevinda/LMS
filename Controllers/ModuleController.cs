using LMS.Data;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels.CourseDetailsVms;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class ModuleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<Module> _modules;

        public ModuleController(ApplicationDbContext context)
        {
            _context = context;
            _modules = new Repository<Module>(context);
        }


        [HttpGet]
        public IActionResult Add(int CourseId)
        {
            var model = new ModuleVm { CourseId= CourseId };
            return View("Add",model);
        }
       
        [HttpPost]
        public async Task<IActionResult> Add(ModuleVm vm)
        {
            /*
                pass html for new page, to append this module to 
            course view,

            how to do it, pass single one or get all modules and pass ?
             */

            if (ModelState.IsValid)
            {
                var module = new Module()
                {
                    CourseId = vm.CourseId,
                    Description = vm.Description,
                    Name = vm.Name,
                };

                await _modules.AddAsync(module);

                ModuleVm moduleVm = new ModuleVm()
                {
                    CourseId = module.CourseId,
                    Description = module.Description,
                    Name = module.Name,
                    Id = module.ModuleId,
                    // List of Content
                };

                string html_ = Helper.RenderRazorViewToString(this, "Partials/_ModulePartial", moduleVm);
                return Json(new { isValid = true, html = html_, moduleId = module.ModuleId });
            }

            string form_html = Helper.RenderRazorViewToString(this, "Add", vm);
            return Json(new { isValid = false, html = form_html });

        }


    }
}
