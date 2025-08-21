using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels
{
    public class Step1ViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
