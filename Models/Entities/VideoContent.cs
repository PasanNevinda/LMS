using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class VideoContent: ContentItem
    {
        [Required]
        public DateTime Duration { get; set; } // Duration of the video content

    }
}
