using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data
{
    //for identity system to recognize the ApplicationUser class, which is derived from IdentityUser
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for the entities

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // primar key , foreign key, and other configurations

        }
    }
}
