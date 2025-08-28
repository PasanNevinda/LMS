using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class DocumentContent: ContentItem
    {
        public int PageCount { get; set; }
    }
}
