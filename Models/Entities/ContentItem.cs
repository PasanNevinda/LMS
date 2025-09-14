using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public  class ContentItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateTime { get; set; } = DateTime.UtcNow;

        public string FilePath { get; set; }     // local path
        public string StageName { get; set; }
        public int OrderNo { get; set; } = 0;


        // Fks
        [Required, ForeignKey(nameof(Module))]
        public int ModuleId { get; set; }


        // Navigation Properties
        public Module Module { get; set; }
    }
}
