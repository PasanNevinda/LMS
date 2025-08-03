using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Teacher: ApplicationUser
    {
        [Key, ForeignKey(nameof(User))]
        public string TeacherId { get; set; }

        // may replace with a more complex type later (string -> EducationQualification Entity)
        public ICollection<string> EducationalQualifications { get; set; } = new List<string>();

        // Navigation Properties
        public ApplicationUser User { get; set; }
        public ICollection<Subject> TeachingSubjects { get; set; } = new List<Subject>();
        public ICollection<Grade> TeachingGrades { get; set; } = new List<Grade>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();


    }
}
