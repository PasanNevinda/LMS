namespace LMS.ViewModels.Student_ViewModles
{
    public class CourseCardVm
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Ratings { get; set; }
        public int NoOfRatings { get; set; }
        public DateTime TotalHours { get; set; }
        public string CourseImage { get; set; } = string.Empty;
    }
}
