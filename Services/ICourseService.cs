using LMS.Models.Entities;
using LMS.ViewModels.CourseDetailsVms;
using LMS.ViewModels.CourseEditVM;

namespace LMS.Services
{
    public interface ICourseService
    {
        Task<Course> CreateCourseAsync(string name, string teacherId);
        Task<Course> GetCourseByIdAsync(int courseId);
        Task<bool> UpdateCourseDetailsAsync(Step2ViewModel model, string teacherId);
        Task<bool> SubmitCourseForReviewAsync(int courseId, string teacherId);
        Task<List<Course>> GetTeacherCoursesAsync(string teacherId);
        Task<List<Course>> GetPendingCoursesAsync();
        Task<bool> ReviewCourseAsync(int courseId, bool approved, string reviewNotes, string adminId);
        Task<bool> PublishCourseAsync(int courseId, string teacherId);
        Task<List<Category>> GetCategoriesAsync();

        Task<bool> DeleteCourseAsync(int courseId, string teacherId);

        Task<Module> CreateModuleAsync(ModuleVm moduleViewModel);

        Task<Module> GetModuleByIdAsync(int moduleId);
        Task<Module> UpdateModuleAsync(ModuleVm vm);
    }
}
