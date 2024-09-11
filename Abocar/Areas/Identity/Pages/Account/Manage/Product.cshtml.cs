using Abocar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class ProductModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProductModel (UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager; 
            _context = context;
            products = new Product(); // Initialize the cartItem list
            productImages = new ProductImage();
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public Product products;
        public ProductImage productImages;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (User.IsInRole("Administrator"))
                {
                    var products = await _context.Products.ToListAsync();
                    var images = await _context.ProductImages.ToListAsync();
                    var ratings = await _context.Review.ToListAsync();

                    if (products.Any())
                    {
                        ViewData["Vendor's Products"] = products;
                        ViewData["ProductImages"] = images;
                        ViewData["Ratings"] = ratings;
                    }
                    return Page();
                }
                else if (User.IsInRole("Vendor"))
                {
                    var vendor = await _context.Vendors
                        .Where(x => x.UserId == user.Email)
                        .FirstOrDefaultAsync();

                    if (vendor != null)
                    {
                        var products = await _context.Products
                            .Where(x => x.VendorId == vendor.Id)
                            .ToListAsync();
                        var images = await _context.ProductImages.ToListAsync();
                        var ratings = await _context.Review.ToListAsync();

                        if (products.Any())
                        {
                            ViewData["Vendor's Products"] = products;
                            ViewData["ProductImages"] = images;
                            ViewData["Ratings"] = ratings;
                        }
                    }
                }
            }
            return Page();
        }

    }
}
