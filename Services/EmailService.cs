using LMS.Models.Entities;

namespace LMS.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendCourseReviewNotificationAsync(Course course, bool approved, string reviewNotes)
        {
            Console.WriteLine($"Email Notification: Course '{course.Name}' has been submitted for review.");
        }

        public Task SendCourseSubmittedNotificationAsync(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
