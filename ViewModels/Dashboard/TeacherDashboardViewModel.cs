using LMS.Models.Entities;

namespace LMS.ViewModels.Dashboard
{
    public class TeacherCourseViewModel
    {
        public List<CourseVm> DraftCourses { get; set; } = new();
        public List<CourseVm> PendingCourses { get; set; } = new();
        public List<CourseVm> ApprovedCourses { get; set; } = new();
        public List<CourseVm> RejectedCourses { get; set; } = new();
        public List<CourseVm> PublishedCourses { get; set; } = new();
    }
}
