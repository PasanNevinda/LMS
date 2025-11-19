using NuGet.Packaging.Signing;

namespace LMS.ViewModels.Student_ViewModles
{
    public class CartItemVm
    {
        public int CartItemId { get; set; }
        public string CourseName { get; set; }
        public int CourseId { get; set; }
        public string CourseImage {  get; set; }
        public string TeacherName {  get; set; }
        public int CourseDurationInMinutes { get; set; }
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
    }
}
