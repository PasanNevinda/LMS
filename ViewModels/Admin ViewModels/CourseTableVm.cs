using LMS.Helpers;
using LMS.Models.Entities;
using Microsoft.Identity.Client;

namespace LMS.ViewModels.Admin_ViewModels
{
    public class CourseTableVm
    {
        public List<CourseDetailVm> Courses { get; set; } = new List<CourseDetailVm>();
        public Pager Pager { get; set; }

       
    }
}
