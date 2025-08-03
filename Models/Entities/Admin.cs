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
        [Key, ForeignKey(nameof(User))]
        public string AdminId { get; set; }

        public AdminLevel AdminLevel { get; private set; }
        public DateTime? LastActiveTime { get; set; }
        
        // Navigation Properties
        public ApplicationUser User { get; set; }

    }
}
