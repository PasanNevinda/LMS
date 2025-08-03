using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDay { get; set; }

        public DateTime RegistrationTime = DateTime.UtcNow;
        public string? Biography { get; set; }

        
        // temparary holder for file from HTTP POST, Do not be in DB
        [NotMapped]
        public IFormFile? ProfileImg { get; set; } 

        public string? ProfileImgPath { get; set; }
    }
}
