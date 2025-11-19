using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public enum AdminLevel
    {
        MAIN_ADMIN = 0,
        SUB_ADMIN = 1,
    }

    public class Admin: ApplicationUser
    {
       
        public AdminLevel AdminLevel { get; private set; }
        public DateTime? LastActiveTime { get; set; }

        public Boolean IsMainAdmin { get; set; } = false;

        public ICollection<Course> ReviewedCourses { get; set; } = new List<Course>();

        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }

        // Navigation Properties
        //public ApplicationUser User { get; set; }

    }
}
