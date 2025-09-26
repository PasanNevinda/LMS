using LMS.Models.Entities;

namespace LMS.ViewModels.Admin_ViewModels
{
    public class CourseDetailVm
    {
        public string Name { get; set; } = string.Empty;
        public string TeacherEmail { get; set; } = string.Empty;
        public string TeacherName {  get; set; } = string.Empty;
        public string Language {  get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Submitted { get; set; }
        public decimal Price { get; set; }

        public CourseDetailVm(Language language, CourseStatus status)
        {
            if (language == Models.Entities.Language.Sinhala)
                Language = "Sinhala";
            if (language == Models.Entities.Language.English)
                Language = "English";
            if (language == Models.Entities.Language.Tamil)
                Language = "Tamil";

            if (status == CourseStatus.Published)
                Status = "Published";
            if (status == CourseStatus.Pending)
                Status = "Pending";
            if (status == CourseStatus.Rejected)
                Status = "Rejected";
            if (status == CourseStatus.Approved)
                Status = "Approved";
            if (status == CourseStatus.Draft)
                Status = "Draft";
        }
    }
}
