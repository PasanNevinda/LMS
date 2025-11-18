using LMS.Helpers;

namespace LMS.ViewModels.Student_ViewModles
{
    public class EnrollCourseVm
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int NoOfLessions { get; set; }
        public int Totalminitues { get; set; }
        public string CourseImage { get; set; } = string.Empty;
    }

    public class StudentDashboardVm
    {
        public List<EnrollCourseVm> Courses { get; set; } = new List<EnrollCourseVm>();
        public Pager Pager { get; set; }
    }
}
