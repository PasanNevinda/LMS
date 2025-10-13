using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace LMS.Models.Entities
{
    public enum Language
    {
        [Display(Name = "Sinhala")]
        Sinhala = 0,

        [Display(Name = "Tamil")]
        Tamil = 1,

        [Display(Name = "English")]
        English = 2,
    }


    public enum CourseStatus{ Draft, Pending, Rejected, Approved, Published }

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

        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        // Tags for the course (implemented later)

        public string Curriculum {  get; set; } = string.Empty;
        public string TargetAudiance {  get; set; } = string.Empty;
        public string CourseImage {  get; set; } = string.Empty;
        public string PromotionVideo {  get; set; } = string.Empty;

        [Range(0, 5)]
        public decimal Rating { get; set; } = 0;

        public decimal Price { get; set; }



        /*// Fks
        [Required, ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [Required, ForeignKey(nameof(Grade))]*/

        [Required, ForeignKey(nameof(Teacher))]
        public string TeacherId { get; set; }


        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public string ReviewNotes { get; set; } = string.Empty;
        public string? ReviewedById { get; set; } 
        public DateTime? ReviewedAt { get; set; } = null;
        public Admin? ReviewedBy { get; set; }

        public ICollection<PreviewCourseQ_A> PreviewCourseQ_As { get; set; } = new List<PreviewCourseQ_A>();

        public Teacher Teacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
