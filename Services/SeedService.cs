using System.Linq.Expressions;
using LMS.Data;
using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace LMS.Services
{
    public class SeedService
    {
        public static async Task SeedDbAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                // ensure database is created
                logger.LogInformation("Ensuring database is created...");
                await context.Database.EnsureCreatedAsync();

                // add roles
                string[] roles = { "Admin", "Teacher", "Student" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        logger.LogInformation($"Creating role: {role}");
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // seed admin user
                var adminEmail = "mainadmin@xyz.com";
                var adminPassword = "Admin123@";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    logger.LogInformation($"Creating admin user: {adminEmail}");
                    adminUser = new Admin
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Main Admin",
                        RegistrationTime = DateTime.UtcNow,
                        IsMainAdmin = true,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Admin user created successfully.");
                        await userManager.AddToRoleAsync(adminUser, "Admin");

                        await context.SaveChangesAsync();
                        logger.LogInformation("Main Admin user created and added to admins table.");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Admin user already exists.");
                    adminUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(adminUser);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }

    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // --- Categories ---
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category {Name = "Programming", Description = "Learn programming languages" , IsActive=true},
                    new Category { Name = "Design", Description = "Learn design principles", IsActive=true },
                    new Category {Name = "Mathematics", Description = "Learn math topics" , IsActive = true},
                    new Category { Name = "Art", Description = "Learn art topics" , IsActive = true},
                    new Category { Name = "Science", Description = "Learn science topics" , IsActive = true},
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // --- Courses ---
            if (!context.Courses.Any())
            {
                var teacher1 = context.Teachers.First();
                var programmingCategory = context.Categories.First(c => c.Name == "Programming");
                var mathCategory = context.Categories.First(c => c.Name == "Mathematics");
                var artCategory = context.Categories.First(c => c.Name == "Art");
                var scienceCategory = context.Categories.First(c => c.Name == "Science");
                var firstAdmin = context.Admins.First();

                var courses = new[]
                {
                    new Course
                    {
                        Name = "C# Basics",
                        Description = "Learn the basics of C#",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = programmingCategory.Id,
                        Status = CourseStatus.Draft,
                        Price = 50,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new Course
                    {
                        Name = "Advanced C#",
                        Description = "Advanced topics in C#",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = programmingCategory.Id,
                        Status = CourseStatus.Pending,
                        Price = 75,
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Course
                    {
                        Name = "Calculus 101",
                        Description = "Introduction to Calculus",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = mathCategory.Id,
                        Status = CourseStatus.Draft,
                        Price = 40,
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Course
                    {
                        Name = "Art 101",
                        Description = "Introduction to Art",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        Status = CourseStatus.Pending,
                        Price = 40,
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                    },
                    new Course
                    {
                        Name = "Science 101",
                        Description = "Introduction to Science",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = scienceCategory.Id,
                        Status = CourseStatus.Draft,
                        Price = 40,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Course
                    {
                        Name = "Calculus 102",
                        Description = "Introduction to Calculus",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = mathCategory.Id,
                        Status = CourseStatus.Pending,
                        Price = 40,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Course
                    {
                        Name = "C# Basics 2",
                        Description = "Learn the basics of C#",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = programmingCategory.Id,
                        Status = CourseStatus.Published,
                        Price = 50,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),   
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),   
                        ReviewedById = firstAdmin.Id,
                        ReviewedBy = firstAdmin,
                        ReviewedAt = DateTime.UtcNow.AddDays(-2),  
                        ReviewNotes = "Approved for publishing"
                    },
                    new Course
                    {
                        Name = "Science Basics 2",
                        Description = "Learn the basics of Science",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = scienceCategory.Id,
                        Status = CourseStatus.Published,
                        Price = 50,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),
                        ReviewedById = firstAdmin.Id,
                        ReviewedBy = firstAdmin,
                        ReviewedAt = DateTime.UtcNow.AddDays(-2),
                        ReviewNotes = "Approved for publishing"
                    },
                    new Course
                    {
                        Name = "C# Basics 3",
                        Description = "Learn the basics of C#",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = programmingCategory.Id,
                        Status = CourseStatus.Approved,
                        Price = 50,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),   // example creation time
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),   // example last update
                        ReviewedById = firstAdmin.Id,
                        ReviewedBy = firstAdmin,
                        ReviewedAt = DateTime.UtcNow.AddDays(-2),  // example review time
                        ReviewNotes = "Approved for publishing"
                    },
                    new Course
                    {
                        Name = "C# 4",
                        Description = "Learn the basics of C#",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = programmingCategory.Id,
                        Status = CourseStatus.Rejected,
                        Price = 50,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),   // example creation time
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),   // example last update
                        ReviewedById = firstAdmin.Id,
                        ReviewedBy = firstAdmin,
                        ReviewedAt = DateTime.UtcNow.AddDays(-2),  // example review time
                        ReviewNotes = "Not Suitable for publishing for publishing"
                    },
                    new Course
                    {
                        Name = "C# 3",
                        Description = "Learn the basics of C#",
                        Language = Language.English,
                        TeacherId = teacher1.Id,
                        CategoryId = programmingCategory.Id,
                        Status = CourseStatus.Rejected,
                        Price = 50,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),   // example creation time
                        UpdatedAt = DateTime.UtcNow.AddDays(-1),   // example last update
                        ReviewedById = firstAdmin.Id,
                        ReviewedBy = firstAdmin,
                        ReviewedAt = DateTime.UtcNow.AddDays(-2),  // example review time
                        ReviewNotes = "Rejected"
                    },

                };

                context.Courses.AddRange(courses);
                context.SaveChanges();
            }

            // --- Students ---
            if (!context.Students.Any())
            {
                var student1 = new Student { Id = Guid.NewGuid().ToString(), UserName = "student1@test.com", Email = "student1@test.com" };
                var student2 = new Student { Id = Guid.NewGuid().ToString(), UserName = "student2@test.com", Email = "student2@test.com" };

                context.Students.AddRange(student1, student2);
                context.SaveChanges();
            }

            // --- Enrollments (Many-to-Many: Student ↔ Course) ---
            if (!context.Enrollments.Any())
            {
                var student1 = context.Students.First();
                var student2 = context.Students.Skip(1).First();
                var course1 = context.Courses.Where(c => c.Status == CourseStatus.Published).First();
                var course2 = context.Courses.Skip(1).First();

                var enrollments = new[]
                {
                    new Enrollment { StudentId = student1.Id, CourseId = course1.CourseId },
                    new Enrollment { StudentId = student1.Id, CourseId = course2.CourseId },
                    new Enrollment { StudentId = student2.Id, CourseId = course2.CourseId }
                };

                context.Enrollments.AddRange(enrollments);
                context.SaveChanges();
            }

            // --- Modules (One-to-Many: Course → Module) ---
            if (!context.Modules.Any())
            {
                var course1 = context.Courses.Where(c => c.Status == CourseStatus.Published || c.Status == CourseStatus.Approved).First();
                var course2 = context.Courses.Where(c => c.Status == CourseStatus.Published || c.Status == CourseStatus.Approved).Skip(1).First();

                var modules = new[]
                {
                    new Module { Name = "Module 1 - C# Basics", CourseId = course1.CourseId, OrderNo = 1 },
                    new Module { Name = "Module 2 - C# Basics", CourseId = course1.CourseId, OrderNo = 2 },
                    new Module { Name = "Module 1 - Advanced C#", CourseId = course2.CourseId, OrderNo = 1 }
                };

                context.Modules.AddRange(modules);
                context.SaveChanges();
            }

            // --- ContentItems (One-to-Many: Module → ContentItem) ---
            if (!context.ContentItems.Any())
            {
                var module1 = context.Modules.First();

                var contents = new[]
                {
                    new ContentItem { Description = "C# Introduction Video", ModuleId = module1.ModuleId, FilePath = "/videos/csharp_intro.mp4", StageName = "Video", OrderNo = 1 },
                    new ContentItem { Description = "C# Basics PDF", ModuleId = module1.ModuleId, FilePath = "/docs/csharp_basics.pdf", StageName = "Document", OrderNo = 2 }
                };

                context.ContentItems.AddRange(contents);
                context.SaveChanges();
            }

            // --- Exams (One-to-Many: Course → Exam) ---
            if (!context.Exams.Any())
            {
                var course1 = context.Courses.Where(c => c.Status == CourseStatus.Published || c.Status == CourseStatus.Approved).First();

                var exams = new[]
                {
                    new Exam { Name = "C# Basics Exam", CourseId = course1.CourseId, StartTime = DateTime.UtcNow.AddDays(1) }
                };

                context.Exams.AddRange(exams);
                context.SaveChanges();
            }
        }

        public static async Task SeedTeachersAsync(IServiceProvider services, List<string> emails)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Teacher"))
            {
                await roleManager.CreateAsync(new IdentityRole("Teacher"));
            }
            int i = 1;

            foreach (var t in emails)
            {
                var teacherEmail = t;
                var teacher = await userManager.FindByEmailAsync(teacherEmail);

                if (teacher == null)
                {
                    teacher = new Teacher
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = teacherEmail,
                        Email = teacherEmail,
                        FullName = "Teacher " + i,
                        EmailConfirmed = true
                    };
                    i++;
                    var result = await userManager.CreateAsync(teacher, "Teacher@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(teacher, "Teacher");
                    }
                    else
                    {
                        throw new Exception("Failed to create teacher: " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

    }
}


