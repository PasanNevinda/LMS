using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int OrderNo { get; set; }


        // Module Quizzes will be implemented later

        // Fks
        [Required, ForeignKey(nameof(Course))]
        public int CourseId { get; set; }


        // Navigation Properties
        public Course Course { get; set; }
        public ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();
    }
}
