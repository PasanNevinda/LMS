using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace LMS.Models.Entities
{
    public enum Language
    {
        Sinhala = 0,
        Tamil1 = 1,
        English = 2,
    }

    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;


        public string Description { get; set; } = string.Empty;

        [Required]
        public Language Language { get; set; } = Language.Sinhala;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Tags for the course (implemented later)

        [Range(0, 5)]
        public decimal Rating { get; set; } = 0;

        [Required]
        public decimal Price { get; set; }



        /*// Fks
        [Required, ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [Required, ForeignKey(nameof(Grade))]*/
        public int GradeId { get; set; }

        [Required, ForeignKey(nameof(Teacher))]
        public string TeacherId { get; set; }



        // Navigation Properties

        //public Subject Subject { get; set; }
       // public Grade Grade { get; set; }

        public ICollection<PreviewCourseQ_A> PreviewCourseQ_As { get; set; } = new List<PreviewCourseQ_A>();

        public Teacher Teacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
