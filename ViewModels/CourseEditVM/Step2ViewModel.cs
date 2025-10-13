using LMS.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.CourseEditVM
{
    public class Step2ViewModel
    {
        public int CourseId { get; set; }

      
        public string? Name { get; set; }

        public int? CategoryId { get; set; }

        [StringLength(100)]
        [Display(Name ="Course Description")]
        public string? Description { get; set; }

        public Language Language { get; set; }

        //[Display(Name = "Promotion Video URL")]
        //[Url(ErrorMessage = "Please enter a valid URL")]
        //public string PromotionVideoUrl { get; set; }

        public string? TargetAudiance { get; set; }

        public CourseStatus Status { get; set; } 

        [Display(Name = "Course Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal Price { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();

        [Display(Name = "Display Image")]
        public IFormFile? DisplayImage { get; set; }
        public string ImagePath { get; set; } = string.Empty;

    }
}
