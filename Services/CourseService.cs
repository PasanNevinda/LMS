using LMS.Data;
using LMS.Models.Entities;
using LMS.ViewModels.CourseDetailsVms;
using LMS.ViewModels.CourseEditVM;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{

    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public CourseService(ApplicationDbContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Course> CreateCourseAsync(string name, string teacherId)
        {
            var course = new Course
            {
                Name = name,
                TeacherId = teacherId,
                Status = CourseStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Module> CreateModuleAsync(ModuleVm moduleViewModel)
        {
            var module = new Module
            {
                Name = moduleViewModel.Name,
                Description = moduleViewModel.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CourseId = moduleViewModel.CourseId,
            };
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteCourseAsync(int courseId, string teacherId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null)
                return false;
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.ContentItems)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<Module> GetModuleByIdAsync(int moduleId)
        {
            return await _context.Modules
                .Include(m => m.ContentItems)
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId);
        }

        public async Task<List<Course>> GetPendingCoursesAsync(int count=0)
        {
            if(count > 0)
            {        return await _context.Courses
                    .Include(c => c.Teacher)
                    .Include(c => c.Category)
                    .Where(c => c.Status == CourseStatus.Pending)
                    .OrderBy(c => c.UpdatedAt)
                    .Take(count)
                    .ToListAsync();
            }

            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .Where(c => c.Status == CourseStatus.Pending)
                .OrderBy(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task<List<Course>> GetTeacherCoursesAsync(string teacherId)
        {
            return await _context.Courses
               .Include(c => c.Category)
               .Where(c => c.TeacherId == teacherId)
               .OrderByDescending(c => c.UpdatedAt)
               .ToListAsync();
        }

        public Task<bool> PublishCourseAsync(int courseId, string teacherId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReviewCourseAsync(int courseId, bool approved, string reviewNotes, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SubmitCourseForReviewAsync(int courseId, string teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null || course.Status != CourseStatus.Draft)
                return false;

            course.Status = CourseStatus.Pending;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            //await _emailService.SendCourseSubmittedNotificationAsync(course);
            return true;
        }

        public async Task<bool> UpdateCourseDetailsAsync(Step2ViewModel model, string teacherId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == model.CourseId && c.TeacherId == teacherId);

            if( course == null || course.Status != CourseStatus.Draft)
                return false;

            if (!string.IsNullOrWhiteSpace(model.Name))
                course.Name = model.Name;

            if (!string.IsNullOrWhiteSpace(model.Description))
                course.Description = model.Description;

            if (!string.IsNullOrWhiteSpace(model.Curriculum))
                course.Curriculum = model.Curriculum;

            if (!string.IsNullOrWhiteSpace(model.TargetAudiance))
                course.TargetAudiance = model.TargetAudiance;

            // category: only set if user selected one
            if (model.CategoryId.HasValue)
                course.CategoryId = model.CategoryId.Value;

            // Price might be 0 for free — decide how you want to treat zero
            course.Price = model.Price;

            if (!string.IsNullOrWhiteSpace(model.ImagePath))
                course.CourseImage = model.ImagePath;

            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Module> UpdateModuleAsync(ModuleVm vm)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == vm.Id);
            module.Name = vm.Name;
            module.Description = vm.Description;
            module.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return module;
        }
    }
}
