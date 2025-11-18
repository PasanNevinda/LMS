using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Teacher: ApplicationUser
    {
        // may replace with a more complex type later (string -> EducationQualification Entity)
        //public ICollection<string> EducationalQualifications { get; set; } = new List<string>();

        // Navigation Properties
       // public ApplicationUser User { get; set; }
       // public ICollection<Subject> TeachingSubjects { get; set; } = new List<Subject>();
        //public ICollection<Grade> TeachingGrades { get; set; } = new List<Grade>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();

        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }

        public decimal AvailableBalance { get; set; }
        public decimal LifetimeEarnings { get; set; }
        // Teacher - Grade
        // Teacher - Subject many to many relationship need to be implemented later

        public ICollection<InstructorPayout> Payouts { get; set; } = new List<InstructorPayout>();
    }
}
