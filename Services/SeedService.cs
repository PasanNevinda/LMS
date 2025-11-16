using LMS.Data;
using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        // -----------------------------------------------------------------------
        // Helper – create 10 teachers + 4 students if they are missing
        // -----------------------------------------------------------------------
        private static async Task EnsureTeachersAndStudentsAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // ---- ROLES ----
            foreach (var r in new[] { "Teacher", "Student" })
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));

            // ---- TEACHERS (10) ----
            var teacherEmails = Enumerable.Range(1, 10)
                                          .Select(i => $"teacher{i}@lms.com")
                                          .ToList();

            int tIdx = 1;
            foreach (var email in teacherEmails)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var teacher = new Teacher
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = email,
                        Email = email,
                        FullName = $"Teacher {tIdx}",
                        EmailConfirmed = true
                    };
                    var res = await userManager.CreateAsync(teacher, "Teacher@123");
                    if (res.Succeeded) await userManager.AddToRoleAsync(teacher, "Teacher");
                    else throw new Exception(string.Join(", ", res.Errors.Select(e => e.Description)));
                }
                tIdx++;
            }

            // ---- STUDENTS (4) ----
            var studentEmails = Enumerable.Range(1, 4)
                                          .Select(i => $"student{i}@lms.com")
                                          .ToList();

            int sIdx = 1;
            foreach (var email in studentEmails)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var student = new Student
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = email,
                        Email = email,
                        FullName = $"Student {sIdx}",
                        EmailConfirmed = true
                    };
                    var res = await userManager.CreateAsync(student, "Student@123");
                    if (res.Succeeded) await userManager.AddToRoleAsync(student, "Student");
                    else throw new Exception(string.Join(", ", res.Errors.Select(e => e.Description)));
                }
                sIdx++;
            }
        }

        // -----------------------------------------------------------------------
        // MAIN SEED METHOD
        // -----------------------------------------------------------------------
        public static async Task SeedAsync(IServiceProvider services, ApplicationDbContext context)
        {
            // 1. Make sure DB exists
            await context.Database.EnsureCreatedAsync();

            // 2. Teachers + Students (10 teachers, 4 students)
            await EnsureTeachersAndStudentsAsync(services);

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("LMS.DbSeeder");

            // -------------------------------------------------------------------
            // CATEGORIES (unchanged)
            // -------------------------------------------------------------------
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                new Category { Name = "Programming", Description = "Learn programming languages", IsActive = true },
                new Category { Name = "Design",       Description = "Learn design principles",    IsActive = true },
                new Category { Name = "Mathematics",  Description = "Learn math topics",         IsActive = true },
                new Category { Name = "Art",          Description = "Learn art topics",          IsActive = true },
                new Category { Name = "Science",      Description = "Learn science topics",      IsActive = true },
            };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // -------------------------------------------------------------------
            // FETCH COMMON ENTITIES
            // -------------------------------------------------------------------
            var admin = await context.Admins.FirstAsync();               // main admin (created by SeedService)
            var teacher1 = await context.Teachers.OrderBy(t => t.Id).FirstAsync();
            var teacher2 = await context.Teachers.OrderBy(t => t.Id).Skip(1).FirstAsync();
            var progCat = await context.Categories.FirstAsync(c => c.Name == "Programming");
            var mathCat = await context.Categories.FirstAsync(c => c.Name == "Mathematics");
            var scienceCat = await context.Categories.FirstAsync(c => c.Name == "Science");
            var artCat = await context.Categories.FirstAsync(c => c.Name == "Art");

            // -------------------------------------------------------------------
            // COURSES
            // -------------------------------------------------------------------
            if (!context.Courses.Any())
            {
                var now = DateTime.UtcNow;
                var rnd = new Random();

                // ---------- DRAFT (4) – only teacher1 & teacher2, partial data ----------
                var drafts = new List<Course>
            {
                new Course {
                    Name = "Draft – C# Intro (T1)", Description = "Partial draft",
                    Language = Language.English, TeacherId = teacher1.Id,
                    CategoryId = progCat.Id, Status = CourseStatus.Draft,
                    Price = 0, CreatedAt = now.AddDays(-3), UpdatedAt = now.AddDays(-1)
                },
                new Course {
                    Name = "Draft – OOP Basics (T1)", Description = "Partial draft",
                    Language = Language.English, TeacherId = teacher1.Id,
                    CategoryId = progCat.Id, Status = CourseStatus.Draft,
                    Price = 0, CreatedAt = now.AddDays(-4), UpdatedAt = now.AddDays(-2)
                },
                new Course {
                    Name = "Draft – Calculus Intro (T2)", Description = "Partial draft",
                    Language = Language.English, TeacherId = teacher2.Id,
                    CategoryId = mathCat.Id, Status = CourseStatus.Draft,
                    Price = 0, CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-1)
                },
                new Course {
                    Name = "Draft – Physics Basics (T2)", Description = "Partial draft",
                    Language = Language.English, TeacherId = teacher2.Id,
                    CategoryId = scienceCat.Id, Status = CourseStatus.Draft,
                    Price = 0, CreatedAt = now.AddDays(-6), UpdatedAt = now.AddDays(-2)
                }
            };
                context.Courses.AddRange(drafts);
                await context.SaveChangesAsync();

                // ---------- FULL DETAIL COURSES ----------
                var fullCourses = new List<Course>();

                // Helper to create a **full** course
                Action<string, string, Language, string, Category, CourseStatus, decimal> addFull = (
                    name, desc, lang, teacherId, cat, status, price) =>
                {
                    fullCourses.Add(new Course
                    {
                        Name = name,
                        Description = desc,
                        Language = lang,
                        TeacherId = teacherId,
                        CategoryId = cat.Id,
                        Status = status,
                        Price = price,
                        CourseImage = "uploads/Images/courseimg.jpg",
                        PromotionVideo = "uploads/Videos/promvideo.mp4",
                        CreatedAt = now.AddDays(-rnd.Next(5, 15)),
                        UpdatedAt = now.AddDays(-rnd.Next(1, 4)),
                        ReviewedById = admin.Id,
                        ReviewedBy = admin,
                        ReviewedAt = now.AddDays(-rnd.Next(1, 3)),
                        ReviewNotes = status == CourseStatus.Published || status == CourseStatus.Approved
                                      ? "Approved for publishing"
                                      : status == CourseStatus.Rejected
                                        ? "Not suitable – " + rnd.Next(1, 100)
                                        : "Pending review"
                    });
                };

                // ---- 8 PUBLISHED -------------------------------------------------
                for (int i = 1; i <= 8; i++)
                {
                    var t = (i % 2 == 0) ? teacher1.Id : teacher2.Id;
                    var cat = i <= 3 ? progCat : i <= 5 ? mathCat : scienceCat;
                    addFull($"Published Course {i}", $"Full description for published course {i}",
                            Language.English, t, cat, CourseStatus.Published, 49.99m + i);
                }

                // ---- 5 PENDING ---------------------------------------------------
                for (int i = 1; i <= 5; i++)
                {
                    var t = (i % 2 == 0) ? teacher1.Id : teacher2.Id;
                    var cat = i <= 2 ? progCat : artCat;
                    addFull($"Pending Course {i}", $"Awaiting admin review – pending {i}",
                            Language.English, t, cat, CourseStatus.Pending, 39.99m + i);
                }

                // ---- 5 APPROVED --------------------------------------------------
                for (int i = 1; i <= 5; i++)
                {
                    var t = (i % 2 == 0) ? teacher1.Id : teacher2.Id;
                    var cat = i <= 3 ? scienceCat : mathCat;
                    addFull($"Approved Course {i}", $"Ready to publish – approved {i}",
                            Language.English, t, cat, CourseStatus.Approved, 59.99m + i);
                }

                // ---- 5 REJECTED --------------------------------------------------
                for (int i = 1; i <= 5; i++)
                {
                    var t = (i % 2 == 0) ? teacher1.Id : teacher2.Id;
                    var cat = i <= 2 ? progCat : artCat;
                    addFull($"Rejected Course {i}", $"Did not meet quality standards – rejected {i}",
                            Language.English, t, cat, CourseStatus.Rejected, 29.99m + i);
                }

                context.Courses.AddRange(fullCourses);
                await context.SaveChangesAsync();

                // ----------------------------------------------------------------
                // MODULES + CONTENT ITEMS (only for Published & Approved)
                // ----------------------------------------------------------------
                var targetCourses = await context.Courses
                    .Where(c => c.Status == CourseStatus.Published || c.Status == CourseStatus.Approved)
                    .ToListAsync();

                foreach (var course in targetCourses)
                {
                    // 2-4 modules per course
                    int modCount = rnd.Next(2, 5);
                    for (int m = 1; m <= modCount; m++)
                    {
                        var module = new Module
                        {
                            Name = $"Module {m} – {course.Name}",
                            Description = $"Module {m} description",
                            CourseId = course.CourseId,
                            OrderNo = m,
                            CreatedAt = now.AddDays(-rnd.Next(1, 10)),
                            UpdatedAt = now.AddDays(-rnd.Next(1, 3))
                        };
                        context.Modules.Add(module);
                        await context.SaveChangesAsync();

                        // 2-5 content items per module (mix of Video & Document)
                        int items = rnd.Next(2, 6);
                        for (int ci = 1; ci <= items; ci++)
                        {
                            bool isVideo = ci % 2 == 0;
                            context.ContentItems.Add(new ContentItem
                            {
                                Description = isVideo ? $"Video lesson {ci}" : $"Document {ci}",
                                FilePath = isVideo ? "uploads/Videos/coursevideo.mp4" : "uploads/Documents/coursedoc.pdf",
                                StageName = isVideo ? "Video" : "Document",
                                Type = isVideo ? "Video" : "Document",
                                OrderNo = ci,
                                ModuleId = module.ModuleId,
                                CreatedAt = now,
                                UpdateTime = now
                            });
                        }
                    }
                }
                await context.SaveChangesAsync();

                // ----------------------------------------------------------------
                // ENROLLMENTS (4 students → random Published courses)
                // ----------------------------------------------------------------
                var students = await context.Students.Take(4).ToListAsync();
                var publishedCourses = await context.Courses
                    .Where(c => c.Status == CourseStatus.Published)
                    .ToListAsync();

                foreach (var s in students)
                {
                    // each student enrolls in 2-4 random published courses
                    var enrollCount = rnd.Next(2, 5);
                    var chosen = publishedCourses.OrderBy(_ => Guid.NewGuid()).Take(enrollCount);
                    foreach (var c in chosen)
                    {
                        if (!context.Enrollments.Any(e => e.StudentId == s.Id && e.CourseId == c.CourseId))
                            context.Enrollments.Add(new Enrollment { StudentId = s.Id, CourseId = c.CourseId });
                    }
                }
                await context.SaveChangesAsync();

                // ----------------------------------------------------------------
                // EXAMS (one per Published/Approved course)
                // ----------------------------------------------------------------
                foreach (var c in targetCourses)
                {
                    context.Exams.Add(new Exam
                    {
                        Name = $"Final Exam – {c.Name}",
                        CourseId = c.CourseId,
                        StartTime = now.AddDays(rnd.Next(2, 10))
                    });
                }
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeded with all requested data.");
            }
        }
    }

    //public static class DbSeeder
    //{
    //    public static void Seed(ApplicationDbContext context)
    //    {
    //        context.Database.EnsureCreated();

    //        // --- Categories ---
    //        if (!context.Categories.Any())
    //        {
    //            var categories = new[]
    //            {
    //                new Category {Name = "Programming", Description = "Learn programming languages" , IsActive=true},
    //                new Category { Name = "Design", Description = "Learn design principles", IsActive=true },
    //                new Category {Name = "Mathematics", Description = "Learn math topics" , IsActive = true},
    //                new Category { Name = "Art", Description = "Learn art topics" , IsActive = true},
    //                new Category { Name = "Science", Description = "Learn science topics" , IsActive = true},
    //            };
    //            context.Categories.AddRange(categories);
    //            context.SaveChanges();
    //        }

    //        // --- Courses ---
    //        if (!context.Courses.Any())
    //        {
    //            var teacher1 = context.Teachers.First();
    //            var programmingCategory = context.Categories.First(c => c.Name == "Programming");
    //            var mathCategory = context.Categories.First(c => c.Name == "Mathematics");
    //            var artCategory = context.Categories.First(c => c.Name == "Art");
    //            var scienceCategory = context.Categories.First(c => c.Name == "Science");
    //            var firstAdmin = context.Admins.First();

    //            var courses = new[]
    //            {
    //                new Course
    //                {
    //                    Name = "C# Basics",
    //                    Description = "Learn the basics of C#",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = programmingCategory.Id,
    //                    Status = CourseStatus.Draft,
    //                    Price = 50,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
    //                },
    //                new Course
    //                {
    //                    Name = "Advanced C#",
    //                    Description = "Advanced topics in C#",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = programmingCategory.Id,
    //                    Status = CourseStatus.Pending,
    //                    Price = 75,
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
    //                    CreatedAt = DateTime.UtcNow.AddDays(-10)
    //                },
    //                new Course
    //                {
    //                    Name = "Calculus 101",
    //                    Description = "Introduction to Calculus",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = mathCategory.Id,
    //                    Status = CourseStatus.Draft,
    //                    Price = 40,
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
    //                    CreatedAt = DateTime.UtcNow.AddDays(-10)
    //                },
    //                new Course
    //                {
    //                    Name = "Art 101",
    //                    Description = "Introduction to Art",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    Status = CourseStatus.Pending,
    //                    Price = 40,
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
    //                    CreatedAt = DateTime.UtcNow.AddDays(-10),
    //                },
    //                new Course
    //                {
    //                    Name = "Science 101",
    //                    Description = "Introduction to Science",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = scienceCategory.Id,
    //                    Status = CourseStatus.Draft,
    //                    Price = 40,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
    //                },
    //                new Course
    //                {
    //                    Name = "Calculus 102",
    //                    Description = "Introduction to Calculus",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = mathCategory.Id,
    //                    Status = CourseStatus.Pending,
    //                    Price = 40,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
    //                },
    //                new Course
    //                {
    //                    Name = "C# Basics 2",
    //                    Description = "Learn the basics of C#",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = programmingCategory.Id,
    //                    Status = CourseStatus.Published,
    //                    Price = 50,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),   
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),   
    //                    ReviewedById = firstAdmin.Id,
    //                    ReviewedBy = firstAdmin,
    //                    ReviewedAt = DateTime.UtcNow.AddDays(-2),  
    //                    ReviewNotes = "Approved for publishing"
    //                },
    //                new Course
    //                {
    //                    Name = "Science Basics 2",
    //                    Description = "Learn the basics of Science",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = scienceCategory.Id,
    //                    Status = CourseStatus.Published,
    //                    Price = 50,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
    //                    ReviewedById = firstAdmin.Id,
    //                    ReviewedBy = firstAdmin,
    //                    ReviewedAt = DateTime.UtcNow.AddDays(-2),
    //                    ReviewNotes = "Approved for publishing"
    //                },
    //                new Course
    //                {
    //                    Name = "C# Basics 3",
    //                    Description = "Learn the basics of C#",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = programmingCategory.Id,
    //                    Status = CourseStatus.Approved,
    //                    Price = 50,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),   // example creation time
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),   // example last update
    //                    ReviewedById = firstAdmin.Id,
    //                    ReviewedBy = firstAdmin,
    //                    ReviewedAt = DateTime.UtcNow.AddDays(-2),  // example review time
    //                    ReviewNotes = "Approved for publishing"
    //                },
    //                new Course
    //                {
    //                    Name = "C# 4",
    //                    Description = "Learn the basics of C#",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = programmingCategory.Id,
    //                    Status = CourseStatus.Rejected,
    //                    Price = 50,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),   // example creation time
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),   // example last update
    //                    ReviewedById = firstAdmin.Id,
    //                    ReviewedBy = firstAdmin,
    //                    ReviewedAt = DateTime.UtcNow.AddDays(-2),  // example review time
    //                    ReviewNotes = "Not Suitable for publishing for publishing"
    //                },
    //                new Course
    //                {
    //                    Name = "C# 3",
    //                    Description = "Learn the basics of C#",
    //                    Language = Language.English,
    //                    TeacherId = teacher1.Id,
    //                    CategoryId = programmingCategory.Id,
    //                    Status = CourseStatus.Rejected,
    //                    Price = 50,
    //                    CreatedAt = DateTime.UtcNow.AddDays(-5),   // example creation time
    //                    UpdatedAt = DateTime.UtcNow.AddDays(-1),   // example last update
    //                    ReviewedById = firstAdmin.Id,
    //                    ReviewedBy = firstAdmin,
    //                    ReviewedAt = DateTime.UtcNow.AddDays(-2),  // example review time
    //                    ReviewNotes = "Rejected"
    //                },

    //            };

    //            context.Courses.AddRange(courses);
    //            context.SaveChanges();
    //        }

    //        // --- Enrollments (Many-to-Many: Student ↔ Course) ---
    //        if (!context.Enrollments.Any())
    //        {
    //            var student1 = context.Students.First();
    //            var student2 = context.Students.Skip(1).First();
    //            var course1 = context.Courses.Where(c => c.Status == CourseStatus.Published).First();
    //            var course2 = context.Courses.Skip(1).First();

    //            var enrollments = new[]
    //            {
    //                new Enrollment { StudentId = student1.Id, CourseId = course1.CourseId },
    //                new Enrollment { StudentId = student1.Id, CourseId = course2.CourseId },
    //                new Enrollment { StudentId = student2.Id, CourseId = course2.CourseId }
    //            };

    //            context.Enrollments.AddRange(enrollments);
    //            context.SaveChanges();
    //        }

    //        // --- Modules (One-to-Many: Course → Module) ---
    //        if (!context.Modules.Any())
    //        {
    //            var course1 = context.Courses.Where(c => c.Status == CourseStatus.Published || c.Status == CourseStatus.Approved).First();
    //            var course2 = context.Courses.Where(c => c.Status == CourseStatus.Published || c.Status == CourseStatus.Approved).Skip(1).First();

    //            var modules = new[]
    //            {
    //                new Module { Name = "Module 1 - C# Basics", CourseId = course1.CourseId, OrderNo = 1 },
    //                new Module { Name = "Module 2 - C# Basics", CourseId = course1.CourseId, OrderNo = 2 },
    //                new Module { Name = "Module 1 - Advanced C#", CourseId = course2.CourseId, OrderNo = 1 }
    //            };

    //            context.Modules.AddRange(modules);
    //            context.SaveChanges();
    //        }

    //        // --- ContentItems (One-to-Many: Module → ContentItem) ---
    //        if (!context.ContentItems.Any())
    //        {
    //            var module1 = context.Modules.First();

    //            var contents = new[]
    //            {
    //                new ContentItem { Description = "C# Introduction Video", ModuleId = module1.ModuleId, FilePath = "/videos/csharp_intro.mp4", StageName = "Video", OrderNo = 1 },
    //                new ContentItem { Description = "C# Basics PDF", ModuleId = module1.ModuleId, FilePath = "/docs/csharp_basics.pdf", StageName = "Document", OrderNo = 2 }
    //            };

    //            context.ContentItems.AddRange(contents);
    //            context.SaveChanges();
    //        }

    //        // --- Exams (One-to-Many: Course → Exam) ---
    //        if (!context.Exams.Any())
    //        {
    //            var course1 = context.Courses.Where(c => c.Status == CourseStatus.Published || c.Status == CourseStatus.Approved).First();

    //            var exams = new[]
    //            {
    //                new Exam { Name = "C# Basics Exam", CourseId = course1.CourseId, StartTime = DateTime.UtcNow.AddDays(1) }
    //            };

    //            context.Exams.AddRange(exams);
    //            context.SaveChanges();
    //        }
    //    }

    //    public static async Task SeedTeachersAsync(IServiceProvider services, List<string> emails)
    //    {
    //        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    //        if (!await roleManager.RoleExistsAsync("Teacher"))
    //        {
    //            await roleManager.CreateAsync(new IdentityRole("Teacher"));
    //        }
    //        int i = 1;

    //        foreach (var t in emails)
    //        {
    //            var teacherEmail = t;
    //            var teacher = await userManager.FindByEmailAsync(teacherEmail);

    //            if (teacher == null)
    //            {
    //                teacher = new Teacher
    //                {
    //                    Id = Guid.NewGuid().ToString(),
    //                    UserName = teacherEmail,
    //                    Email = teacherEmail,
    //                    FullName = "Teacher " + i,
    //                    EmailConfirmed = true
    //                };
    //                i++;
    //                var result = await userManager.CreateAsync(teacher, "Teacher@123");

    //                if (result.Succeeded)
    //                {
    //                    await userManager.AddToRoleAsync(teacher, "Teacher");
    //                }
    //                else
    //                {
    //                    throw new Exception("Failed to create teacher: " +
    //                        string.Join(", ", result.Errors.Select(e => e.Description)));
    //                }
    //            }
    //        }
    //    }

    //    public static async Task SeedStudentAsync(IServiceProvider services, List<string> emails)
    //    {
    //        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    //        if (!await roleManager.RoleExistsAsync("Student"))
    //        {
    //            await roleManager.CreateAsync(new IdentityRole("Student"));
    //        }
    //        int i = 1;

    //        foreach (var t in emails)
    //        {
    //            var StudentEmail = t;
    //            var student = await userManager.FindByEmailAsync(StudentEmail);

    //            if (student == null)
    //            {
    //                student = new Student
    //                {
    //                    Id = Guid.NewGuid().ToString(),
    //                    UserName = StudentEmail,
    //                    Email = StudentEmail,
    //                    FullName = "Student " + i,
    //                    EmailConfirmed = true
    //                };
    //                i++;
    //                var result = await userManager.CreateAsync(student, "Student@123");

    //                if (result.Succeeded)
    //                {
    //                    await userManager.AddToRoleAsync(student, "Student");
    //                }
    //                else
    //                {
    //                    throw new Exception("Failed to create student: " +
    //                        string.Join(", ", result.Errors.Select(e => e.Description)));
    //                }
    //            }
    //        }
    //    }
    //}
}


