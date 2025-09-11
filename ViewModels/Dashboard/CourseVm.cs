using LMS.Models.Entities;
using System.Security.Policy;

namespace LMS.ViewModels.Dashboard
{
    public class CourseVm
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = "";
        public string ImagePath { get; set; }
        public CourseStatus Status { get; set; }
        public int Progress { get; set; } = 0; // 0..100
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewdAt { get; set; }
    }
}
