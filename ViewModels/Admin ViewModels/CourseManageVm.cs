using LMS.Helpers;
using LMS.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.ViewModels.Admin_ViewModels
{
    public class CourseManageVm
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<SelectListItem> AllowdStatus { get; set; } 

        public CourseTableVm CourseTable { get; set; }

        public CourseStatus? SelectedStatus { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string? SearchString { get; set; }
    }
}
