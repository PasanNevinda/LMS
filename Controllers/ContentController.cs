using LMS.Data;
using LMS.Data.Migrations;
using LMS.Models.Entities;
using LMS.Services;
using LMS.ViewModels;
using LMS.ViewModels.ContentVMs;
using LMS.ViewModels.CourseDetailsVms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
namespace LMS.Controllers
{/*
    public class ContentController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public ContentController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult ContentUpload()
        {
            var model = new ContentUploadVM();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ContentUpload(ContentUploadVM model)
        {

            Console.WriteLine("File Count is " + model.Files.Count);

            if (ModelState.IsValid)
            {

                if (model.Files != null && model.Files.Count > 0)
                {

                    foreach (var file in model.Files)
                    {
                        var c = 1;
                        Console.WriteLine("Count is " + c);
                        c++;
                        if (file.Length > 0)
                        {
                            // create a unique name
                            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var extension = Path.GetExtension(file.FileName);
                            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                            // define path to save
                            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);

                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // save file to disk
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // save file info in DB
                            string mimeType = file.ContentType;
                            var TID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                            var upload = new ContentItem
                            {
                                Title = file.Name,
                                Description = file.Name,
                                CreatedAt = DateTime.Now,
                                TeacherId = TID,
                                FilePath = filePath

                            };
                            _context.ContentItems.Add((upload));



                        }
                    }

                    await _context.SaveChangesAsync();
                }



                return RedirectToAction("ContentUpload");


                Console.WriteLine("Upload Successfull");
                
            }
            return View(model);
        }


    }*/
    public class ContentController : Controller
    {
        private readonly IFileStorage _fileStorage;
        private readonly ApplicationDbContext _db;

        public ContentController(IFileStorage fileStorage, ApplicationDbContext db)
        {
            _fileStorage = fileStorage;
            _db = db;
        }

        [HttpGet]
        public IActionResult ContentUpload()
        {
            var model = new ContentUploadVM {};
            return View("ContentUpload",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContentUpload(ContentUploadVM model)
        {
            if (model.Files == null )
            {
                ModelState.AddModelError("", "Please select one or more files.");
                return View(model);
            }

            var uploadedCount = 0;
            foreach (var file in model.Files)
            {
                try
                {
                    if (file.Length == 0) continue;

                    // Optional: limit file size (e.g. 50MB max)
                    const long fileLimit = 50 * 1024 * 1024;
                    if (file.Length > fileLimit)
                    {
                        ModelState.AddModelError("", $"File '{file.FileName}' exceeds the maximum allowed size.");
                        continue;
                    }

                    // Save file to local storage
                    string relativePath = await _fileStorage.SaveFileAsync(file);
                    var TID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Record metadata in database
                    var record = new ContentItem
                    {
                        Title = Path.GetFileName(file.FileName),
                        FilePath = relativePath,
                        //ContentType = file.ContentType,
                        //Size = file.Length,
                        CreatedAt = DateTime.UtcNow,
                        Description = Path.GetFileName(file.FileName),
                        TeacherId = TID
                    };
                    _db.ContentItems.Add(record);
                    uploadedCount++;
                }
                catch (Exception ex)
                {
                    // Log exception (omitted) and inform user
                    ModelState.AddModelError("", $"Error uploading '{file.FileName}': {ex.Message}");
                }
            }

            await _db.SaveChangesAsync();
            ViewData["Result"] = $"{uploadedCount} file(s) uploaded successfully.";
            return View("ContentUpload");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var fileRecord = await _db.ContentItems.FindAsync(id);
            if (fileRecord == null) return View();

            await _fileStorage.DeleteFileAsync(fileRecord.FilePath);

            _db.ContentItems.Remove(fileRecord);
            await _db.SaveChangesAsync();

            return View();
        }
    }

}
