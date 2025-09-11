using System.ComponentModel.DataAnnotations;
using LMS.Models.Entities;

namespace LMS.ViewModels
{
    public class Step3ViewModel
    {
        
        public string Description { get; set; }

        [Required]
        public Language Language { get; set; }
    }
}
