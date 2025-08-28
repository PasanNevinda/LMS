using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    // <summary>
    // Represents Many to Many relationship between Module and ContentItem.
    public class ModuleContentItem
    {
        public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
        public int OrderNo { get; set; } = 0;

        public string DisplayName { get; set; } = "Display Name";

        // Fks
        [Required, ForeignKey(nameof(Module))]
        public int ModuleId { get; set; }

        [Required, ForeignKey(nameof(ContentItem))]
        public int ContentItemId { get; set; }




        // Navigation Properties
        public Module Module { get; set; }
        public ContentItem ContentItem { get; set; }
    }
}
