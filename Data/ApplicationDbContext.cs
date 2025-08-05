using System.Reflection.Emit;
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
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        public DbSet<PreviewCourseQ_A> PreviewCourseQ_As { get; set; }


        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Exam> Exams { get; set; }


        public DbSet<ContentItem> ContentItems { get; set; }
        public DbSet<VideoContent> VideoContents { get; set; }
        public DbSet<DocumentContent> DocumentContents { get; set; }
        public DbSet<LinkContent> LinkContents { get; set; }

        public DbSet<ModuleContentItem> ModuleContentItems { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Table Per Type (TPT) configuration for User
            builder.Entity<Student>().ToTable("Students");
            builder.Entity<Teacher>().ToTable("Teachers");
            builder.Entity<Admin>().ToTable("Admins");

            // TPT configeration for ContentItem
            builder.Entity<ContentItem>().ToTable("ContentItems");
            builder.Entity<VideoContent>().ToTable("VideoContents");
            builder.Entity<DocumentContent>().ToTable("DocumentContents");
            builder.Entity<LinkContent>().ToTable("LinkContents");

            builder.Entity<Course>()
             .Property(c => c.Price)
             .HasPrecision(8, 2);

            builder.Entity<Course>()
            .Property(c => c.Rating)
            .HasPrecision(2,1);

            // ----  Configuring relationships  --- //

            // Enrollment relationship
            builder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            builder.Entity<Enrollment>().HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Enrollment>().HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                 .OnDelete(DeleteBehavior.NoAction); 

            // ModuleContentItem relationship
            builder.Entity<ModuleContentItem>()
                .HasKey(mci => new { mci.ModuleId, mci.ContentItemId });

            builder.Entity<ModuleContentItem>().HasOne(mci => mci.Module)
                .WithMany(m => m.ModuleContentItems)
                .HasForeignKey(mci => mci.ModuleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ModuleContentItem>().HasOne(mci => mci.ContentItem)
                .WithMany(c => c.ModuleContentItems)
                .HasForeignKey(mci => mci.ContentItemId)
                .OnDelete(DeleteBehavior.NoAction);

            // PreviewCourseQ_A relationship
            builder.Entity<PreviewCourseQ_A>()
                .HasKey(p => new { p.CourseId, p.StudentId });

            builder.Entity<PreviewCourseQ_A>().HasOne(p => p.Course)
                .WithMany(c => c.PreviewCourseQ_As)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PreviewCourseQ_A>().HasOne(p => p.Student)
                .WithMany(s => s.PreviewCourseQ_As)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.NoAction);






        }
    }
}
