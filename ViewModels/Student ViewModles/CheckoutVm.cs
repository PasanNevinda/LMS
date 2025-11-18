using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.Student_ViewModles
{
        public class CheckoutVm
        {
            public string StudentId { get; set; }
            public string StudentName { get; set; }
            public decimal TotalAmount { get; set; }  // Non-editable

            [Required(ErrorMessage = "Card number is required.")]
            [StringLength(12)]
            public string CardNumber { get; set; }

            [Required]
            [Range(1, 12, ErrorMessage = "Invalid month.")]
            public string ExpiryMonth { get; set; }

            [Required]
            [Range(2024, 2100, ErrorMessage = "Invalid expiry year.")]
            public string ExpiryYear { get; set; }

            [Required]
            [StringLength(4, MinimumLength = 3, ErrorMessage = "Invalid CVV.")]
            public string Cvv { get; set; }
        }

}
