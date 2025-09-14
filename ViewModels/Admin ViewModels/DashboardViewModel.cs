using LMS.Models.Entities;

namespace LMS.ViewModels.Admin_ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCourses { get; set; }
        public decimal TotalCourseIncrease { get; set; }

        public int ActiveStudents { get; set; }
        public decimal TotalStudentIncrease { get; set; }

        public int PendingReviews { get; set; }
       
        public decimal TotalRevenue { get; set; }
        public decimal TotalRevenueIncrease { get; set; }

        public List<Course> RecentCourses { get; set; } = new List<Course>();
    }
}
