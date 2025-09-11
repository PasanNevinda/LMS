namespace LMS.Services
{
    public interface IFileStorage
    {
        /// <summary>
        /// Saves the given uploaded file to storage and returns the relative path where it was saved.
        /// </summary>
        Task<string> SaveFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string filePath);
    }
}
