using LMS.Models.Entities;

namespace LMS.Services
{
    public interface IEmailService
    {
        Task SendCourseSubmittedNotificationAsync(Course course);
        Task SendCourseReviewNotificationAsync(Course course, bool approved, string reviewNotes);
    }
}
