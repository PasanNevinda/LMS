namespace LMS.Services
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class LocalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;
        private static readonly string[] _permittedExtensions = { ".pdf", ".docx", ".doc", ".pptx", ".ppt", ".mp4", ".jpg", ".jpeg", ".png" };
        private static readonly string[] _permittedContentTypes = {
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "video/mp4", 
        "image/jpeg",
        "image/png"
    };

        public LocalFileStorage(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            // Strip any path from the submitted file name for security
            var originalFileName = Path.GetFileName(file.FileName);

            // Validate file extension
            var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_permittedExtensions.Contains(ext))
                throw new InvalidOperationException($"File extension '{ext}' is not permitted.");

            // Validate content type
            if (!_permittedContentTypes.Contains(file.ContentType))
                throw new InvalidOperationException($"Content type '{file.ContentType}' is not permitted.");

            // Classify file by extension
            string category = ext switch
            {
                ".jpg" or ".jpeg" or ".png" => "Images",
                ".mp4" => "Videos",
                _ when new[] { ".pdf", ".docx", ".doc", ".pptx", ".ppt" }.Contains(ext) => "Documents",
                _ => "Others"
            };

            // Build the target directory (e.g. UploadedFiles/Documents)
            string basePath = Path.Combine(_env.WebRootPath, "uploads");
            string folderPath = Path.Combine(basePath, category);
            Directory.CreateDirectory(folderPath); // Ensures folder exists

            // Generate a safe unique file name (do not trust original name for storage)
            string safeFileName = Guid.NewGuid().ToString() + ext;
            string fullPath = Path.Combine(folderPath, safeFileName);

            // Copy file to target
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return relative path (relative to wwwroot)
            return Path.Combine("uploads",category, safeFileName).Replace("\\", "/");
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(_env.WebRootPath, filePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }

}
