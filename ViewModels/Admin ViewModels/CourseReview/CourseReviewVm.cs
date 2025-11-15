
using LMS.Models.Entities;

namespace LMS.ViewModels.Admin_ViewModels.CourseReview
{
    public class CourseReviewVm
    {
        public int CourseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public CourseStatus CourseStatus { get; set; }


        public string Description { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public string TargetAudiance { get; set; } = string.Empty;
        public string CourseImage { get; set; } = string.Empty;
        public string PromotionVideo { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string Category {  get; set; } = string.Empty;

        public string TeacherName {  get; set; } = string.Empty;

        public List<ModuleVm> ModuleList = new List<ModuleVm>();

    }
}
