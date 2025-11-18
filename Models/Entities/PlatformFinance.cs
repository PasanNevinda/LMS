using LMS.Helpers;

namespace LMS.Models.Entities
{

    // single row talbe
    public class PlatformFinance
    {
        public int Id { get; set; }
        public decimal TotalCommissionEarned { get; set; }    // lifetime
        public decimal TotalPaidToInstructors { get; set; }   // lifetime
        public decimal CurrentBalance => Money.Round(TotalCommissionEarned - TotalPaidToInstructors);
    }
}
