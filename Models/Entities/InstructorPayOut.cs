namespace LMS.Models.Entities
{

    public enum PayoutStatus { Pending, Paid, Rejected }

    public class InstructorPayout
    {
        public int Id { get; set; }
        public string TeacherId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? PaidAt{ get; set; } 
        public PayoutStatus Status { get; set; }   // e.g. "Pending", "Paid"
        public string BankAccountNo { get; set; }
        // Navigation props
        public Teacher Teacher { get; set; }
    }

}
