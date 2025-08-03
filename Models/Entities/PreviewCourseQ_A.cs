using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    /// <summary>
    /// Represents a preview question and answer for a course.
    /// Many to many relationship between Course and Student.
    /// </summary>
    public class PreviewCourseQ_A
    {
        public string Question { get; set; } = string.Empty;
        public string? Answer { get; set; } = string.Empty;
        public DateTime AskTime { get; set; } = DateTime.UtcNow;
        public DateTime? AnswerTime { get; set; } = null;



        // Fks
        [Required, ForeignKey(nameof(Course))]
        public int CourseId { get; set; }

        [Required, ForeignKey(nameof(Student))]
        public string StudentId { get; set; }


        // Navigation Properties

        public Course Course { get; set; } = null!;
        public Student Student { get; set; } = null!;


    }
}
