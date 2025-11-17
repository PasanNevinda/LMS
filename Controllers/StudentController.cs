using AspNetCoreGeneratedDocument;
using LMS.Data;
using LMS.Models.Entities;
using LMS.ViewModels.Dashboard;
using LMS.ViewModels.Student_ViewModles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NuGet.Packaging.Signing;
using System.ComponentModel;

namespace LMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this._context = context;
            _userManager = userManager;
        }

        public override async Task OnActionExecutionAsync(
        Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context,
        Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
        {
            var userId = _userManager.GetUserId(User);
            int cartCount = 0;

            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                    cartCount = cart.Items.Count;
            }

            ViewData["CartItemCount"] = cartCount;

            await next();
        }

        public IActionResult StudentDashBoard()
        {
            ViewData["CurrentPage"] = "StudentDashBoard";
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> BrowseCourse(int page = 1, int pageSize = 5, int? CategoryId = null, string? Search = null)
        {
            var query = _context.Courses.
                Include(c => c.Category)
                .Include(c => c.Teacher)
                .AsQueryable();

            if (CategoryId.HasValue)
                query = query.Where(c => c.CategoryId == CategoryId.Value);

            if (!string.IsNullOrEmpty(Search))
                query = query.Where(c => c.Name.Contains(Search) || c.Teacher.FullName.Contains(Search));

            query = query.Where(c => c.Status == CourseStatus.Published);
            var totalItems = await query.CountAsync();

            var vm = new CourseBrowse();
            vm.Pager = new Helpers.Pager(totalItems, page, pageSize);
            vm.Courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseCardVm()
                {
                    CourseId = c.CourseId,
                    CourseName = c.Name,
                    TeacherName = c.Teacher.FullName,
                    Price = c.Price,
                    Ratings = c.Rating,
                    CourseImage = c.CourseImage
                }).ToListAsync();

            vm.categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            vm.SearchString = Search;
            vm.SelectedCategoryId = CategoryId;

            ViewData["CurrentPage"] = "BrowseCourse";
            return View(vm);
        }

        public async Task<IActionResult> CourseDetails(int CourseId)
        {
            var course = await _context.Courses
                .Include(c=>c.Category)
                .Include(c => c.Teacher)
                .Include(c=> c.Modules)
                .ThenInclude(m => m.ContentItems)
                .FirstOrDefaultAsync(c => c.CourseId == CourseId);

            if (course == null)
                return NotFound();

            var userId = _userManager.GetUserId(User); 
            bool isInCart = false;

            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    isInCart = cart.Items.Any(i => i.CourseId == CourseId);
                }
            }

            var vm = new CourseDetail()
            {
                CourseId = course.CourseId,
                Name = course.Name,
                Category = course.Category.Name,
                Rating = course.Rating,
                TeacherName = course.Teacher.FullName,
                LastUpdate = course.UpdatedAt,
                Description = course.Description,
                Language = course.Language.ToString(),
                Price = course.Price,
                CourseImage = course.CourseImage,
                PromotionVideo = course.PromotionVideo,
                ModuleVmList = course.Modules.Select(m => new ModuleVm()
                {
                    Name = m.Name,
                    Description= m.Description,
                    Items = m.ContentItems.Select(c => new ModuleItem()
                    {
                        ItemName = c.StageName,
                        ItemType = c.Type,
                        Description = c.Description,
                    }).ToList()
                }).ToList(),

                IntheCart = isInCart,

            };
            

            ViewData["CurrentPage"] = "BrowseCourse";
            return View(vm);
        }



        [Authorize(Roles ="Student")]
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = _userManager.GetUserId(User);
            int CourseId = request.CourseId;

            var course = await _context.Courses.FindAsync(CourseId);
            if (course == null)
            {
                return Json(new { success = false, error = new { message = "Course not found." } });
            }

            // Get or create the user's cart
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            if (cart.Items.Any(i => i.CourseId == CourseId))
            {
                return Json(new { success = false, error = new { message = "Course is already in the cart." } });
            }

            cart.Items.Add(new CartItem
            {
                CourseId = CourseId
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }



        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Cart()
        {
            ViewData["CurrentPage"] = "Cart";

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("StudentDashBoard");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(c => c.Teacher)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(c => c.Modules)
                            .ThenInclude(m => m.ContentItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return View(new CartVm
                {
                    CartId = 0,
                    TotalAmount = 0,
                    Items = new List<CartItemVm>()
                });
            }

            var cartVm = new CartVm
            {
                CartId = cart.Id,
                TotalAmount = cart.Items.Sum(i => i.Course.Price),
                Items = cart.Items.Select(i =>
                {
                    // Sum duration in minutes for Video content items
                    int totalMinutes = i.Course.Modules
                        .SelectMany(m => m.ContentItems)
                        .Where(ci => ci.Type == "Video")
                        .Sum(ci => ci.DurationInMinutes);

                    return new CartItemVm
                    {
                        CartItemId = i.Id,
                        CourseId = i.CourseId,
                        CourseName = i.Course.Name,
                        CourseImage = i.Course.CourseImage,
                        TeacherName = i.Course.Teacher.FullName,
                        CourseDurationInMinutes = totalMinutes,
                        Price = i.Course.Price,
                        Rating = i.Course.Rating
                    };
                }).ToList()
            };

            return View(cartVm);
        }


        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return View("Cart");

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
                return View("Cart");

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");

        }



    }
}
