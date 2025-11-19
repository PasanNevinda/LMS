using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime Duration { get; set; }

        // Fks
        [Required, ForeignKey(nameof(Course))]
        public int CourseId { get; set; }



        // Navigation Properties
        public Course Course { get; set; } = null!;


        // Questino implementation will be done later
    }
}
