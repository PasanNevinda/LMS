using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class VideoContent: ContentItem
    {
        [Required]
        public string VideoUrl { get; set; }
        public DateTime Duration { get; set; } // Duration of the video content

        [NotMapped]
        public IFormFile? VideoFile { get; set; } // Temporary holder for file from HTTP POST, not stored in DB
    }
}
