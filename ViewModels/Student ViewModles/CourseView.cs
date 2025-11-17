using LMS.Models.Entities;

namespace LMS.ViewModels.Student_ViewModles
{
    public class CourseDetail
    {
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public decimal Rating { get; set; }
        public int NoOfRatings { get; set; }
        public int NoOfStudents { get; set; }

        public string TeacherName { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }


        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public Boolean IntheCart { get; set; }


        public string CourseImage { get; set; } = string.Empty;
        public string PromotionVideo { get; set; } = string.Empty;
        public TimeSpan VideoLength { get; set; }


        public List<ModuleVm> ModuleVmList { get; set; } = new List<ModuleVm>();

    }

    public class ModuleVm
    {
        public string Name { get; set; } = string.Empty;

        public List<ModuleItem> Items = new List<ModuleItem>();
        public string Description { get; set; }
    }

    public class ModuleItem
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime VideoLenth { get; set; }

    }
}
