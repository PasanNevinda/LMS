using LMS.Helpers;

namespace LMS.Models.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string StudentId { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal InstructorAmount  => Money.Round(Amount - CommissionAmount);
        public DateTime PaymentTime { get; set; }
        public string TransactionId { get; set; } = string.Empty;

        // Navigation props
        public Course Course { get; set; }
        public Student Student { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

}
