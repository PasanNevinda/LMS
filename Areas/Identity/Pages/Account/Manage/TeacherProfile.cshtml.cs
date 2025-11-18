using LMS.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LMS.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Teacher")]
    public class TeacherProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherProfileModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public BankInfoInputModel BankInfo { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class BankInfoInputModel
        {
            public string BankName { get; set; }
            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not Teacher teacher)
            {
                // Handle unexpected case (e.g., log error or redirect)
                return RedirectToPage("/Error");
            }

            BankInfo = new BankInfoInputModel
            {
                BankName = teacher.BankName,
                AccountName = teacher.AccountName,
                AccountNumber = teacher.AccountNumber,
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is not Teacher teacher)
            {
                // Handle unexpected case (e.g., log error or redirect)
                return RedirectToPage("/Error");
            }

            teacher.BankName = BankInfo.BankName;
            teacher.AccountName = BankInfo.AccountName;
            teacher.AccountNumber = BankInfo.AccountNumber;

            var updateResult = await _userManager.UpdateAsync(teacher);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            TempData["StatusMessage"] = "Bank information updated.";
            return RedirectToPage();
        }
    }
}