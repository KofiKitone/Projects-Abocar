using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Abocar.Models;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Abocar.Areas.Identity.Pages.Account.ExternalLoginModel;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class Wishlist : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public Wishlist(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            civm = new cartItemViewModel();
        }

        public cartItemViewModel civm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var wishlist = await _context.Wishlist
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            if (wishlist != null && wishlist.Any())
            {
                var wishlistViewModel = new List<WishlistViewModel>();

                foreach (var item in wishlist)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(x => x.Id == item.Product);

                    if (product == null)
                    {
                        // Handle the case where the product is not found
                        continue;
                    }

                    var productImage = await _context.ProductImages
                        .FirstOrDefaultAsync(x => x.ProductId == product.Id);

                    var reviews = await _context.Review
                        .Where(x => x.ProductId == product.Id)
                        .ToListAsync();

                    var wishVM = new WishlistViewModel
                    {
                        Product = product,
                        Image = productImage,
                        Review = reviews.Any() ? reviews : new List<Review>()
                    };

                    wishlistViewModel.Add(wishVM);
                }

                ViewData["WishList"] = wishlistViewModel;
            }

            return Page();
        }
        public async Task LoadAsync()
        {
            
        }
          
        public IActionResult OnPostDelete(int Id)
        {
            var wishlist = _context.Wishlist.Where(x => x.Product == Id).FirstOrDefault();
            if (wishlist == null)
            {
                return NotFound();
            }
            _context.Wishlist.Remove(wishlist);
            _context.SaveChanges();
            @TempData["SuccessMessage"] = "Product removed from wishlist succesfully";
            return RedirectToPage();
        }


    }

}
