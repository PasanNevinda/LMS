using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Student: ApplicationUser
    {

        // interested learning area need to be added later

        // Navigation Properties
        //public ApplicationUser User { get; set; }
        public ICollection<PreviewCourseQ_A> PreviewCourseQ_As { get; set; } = new List<PreviewCourseQ_A>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
