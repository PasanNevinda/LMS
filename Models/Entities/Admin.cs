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

        // Navigation Properties
        //public ApplicationUser User { get; set; }

    }
}
