using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    /// <summary>
    ///  Represnet Student enrollment in Course
    ///  Many to Many Relationship between Student and Course
    /// </summary>
    public class Enrollment
    {
        [Required, ForeignKey(nameof(Student))]
        public string StudentId { get; set; }


        [Required, ForeignKey(nameof(Course))]
        public string CourseId { get; set; }

        public DateTime EnrolledTime { get; set; } = DateTime.UtcNow;

        

        // Navigation Properties
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
