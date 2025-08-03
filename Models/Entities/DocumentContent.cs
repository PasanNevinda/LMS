using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class DocumentContent: ContentItem
    {
        [Required]
        public string DocumentUrl { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? DocumentFile { get; set; } // Temporary holder for file from HTTP POST, Do not be in DB

    }
}
