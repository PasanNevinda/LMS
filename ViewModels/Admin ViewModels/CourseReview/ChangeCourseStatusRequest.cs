namespace LMS.ViewModels.Admin_ViewModels.CourseReview
{
    public class ChangeCourseStatusRequest
    {
        public int courseId { get; set; }
        public string status { get; set; } = "";
    }
}