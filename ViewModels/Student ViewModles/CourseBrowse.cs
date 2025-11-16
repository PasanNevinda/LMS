using LMS.Helpers;
using LMS.Models.Entities;

namespace LMS.ViewModels.Student_ViewModles
{
    public class CourseBrowse
    {
       public List<CourseCardVm> Courses = new List<CourseCardVm>();
        public Pager Pager = new Pager();

        public List<Category> categories = new List<Category>();

        public int? SelectedCategoryId { get; set; }
        public string? SearchString { get; set; } = string.Empty;

    }
}
