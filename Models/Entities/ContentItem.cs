using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public  class ContentItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string FilePath { get; set; }     // local path


        // Fks
        [Required, ForeignKey(nameof(Teacher))]
        public string TeacherId { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<ModuleContentItem> ModuleContentItems { get; set; } = new List<ModuleContentItem>();
        public Teacher Teacher { get; set; }

    }
}
