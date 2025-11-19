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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Exam> Exams { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
       

        public DbSet<ContentItem> ContentItems { get; set; }


        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<InstructorPayout> InstructorPayouts { get; set; }
        public DbSet<PlatformFinance> PlatformFinances { get; set; }
        // public DbSet<ContentItem> ContentUploads { get; set; }
        // public DbSet<VideoContent> VideoContents { get; set; }
        //  public DbSet<DocumentContent> DocumentContents { get; set; }
        // public DbSet<LinkContent> LinkContents { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            

            //  builder.Entity<ContentUpload>(entity =>
            /*  {
                  entity.ToTable("ContentItems");
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.Title).IsRequired();
                  entity.Property(e => e.FilePath).IsRequired();
                  entity.Property(e => e.Description).IsRequired();
                  //entity.Property(e => e.ContentType).IsRequired();
                  //entity.Property(e => e.Size).IsRequired();
                  entity.Property(e => e.CreatedAt).IsRequired();
              });*/

            // Table Per Type (TPT) configuration for User
            builder.Entity<Student>().ToTable("Students");
            builder.Entity<Teacher>().ToTable("Teachers");
            builder.Entity<Admin>().ToTable("Admins");

            // TPT configeration for ContentItem
            builder.Entity<ContentItem>().ToTable("ContentItems");
            //builder.Entity<VideoContent>().ToTable("VideoContents");
            //builder.Entity<DocumentContent>().ToTable("DocumentContents");
            //builder.Entity<LinkContent>().ToTable("LinkContents");

            builder.Entity<Course>()
             .Property(c => c.Price)
             .HasPrecision(8, 2);

            builder.Entity<Course>()
            .Property(c => c.Rating)
            .HasPrecision(2,1);


            builder.Entity<Teacher>()
        .Property(t => t.AvailableBalance)
        .HasColumnType("decimal(18,2)");

            builder.Entity<Teacher>()
                .Property(t => t.LifetimeEarnings)
                .HasColumnType("decimal(18,2)");

            builder.Entity<InstructorPayout>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<PaymentTransaction>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<PaymentTransaction>()
                .Property(p => p.CommissionAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<PlatformFinance>()
                .Property(p => p.TotalCommissionEarned)
                .HasColumnType("decimal(18,2)");

            builder.Entity<PlatformFinance>()
                .Property(p => p.TotalPaidToInstructors)
                .HasColumnType("decimal(18,2)");
            // ----  Configuring relationships  --- //

            // Enrollment relationship


            builder.Entity<Enrollment>().HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Enrollment>().HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Enrollment>()
            .HasOne(e => e.PaymentTransaction)
            .WithMany(pt => pt.Enrollments)
            .HasForeignKey(e => e.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Restrict);


            // PaymentTransaction -> Course (many-to-one)
            builder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Course)
                .WithMany()
                .HasForeignKey(pt => pt.CourseId)
                .OnDelete(DeleteBehavior.Restrict);


            //// ModuleContentItem relationship
            //builder.Entity<ModuleContentItem>()
            //    .HasKey(mci => new { mci.ModuleId, mci.ContentItemId });

            //builder.Entity<ModuleContentItem>().HasOne(mci => mci.Module)
            //    .WithMany(m => m.ModuleContentItems)
            //    .HasForeignKey(mci => mci.ModuleId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<ModuleContentItem>().HasOne(mci => mci.ContentItem)
            //    .WithMany(c => c.ModuleContentItems)
            //    .HasForeignKey(mci => mci.ContentItemId)
            //    .OnDelete(DeleteBehavior.NoAction);

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

            builder.Entity<Course>()
                .HasOne(c => c.ReviewedBy)
                .WithMany(a => a.ReviewedCourses)
                .HasForeignKey(c => c.ReviewedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Course)
                .WithMany()
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<PlatformFinance>().HasData(new PlatformFinance
            {
                Id = 1,
                TotalCommissionEarned = 0m,
                TotalPaidToInstructors = 0m
            });

        }
    }
}
