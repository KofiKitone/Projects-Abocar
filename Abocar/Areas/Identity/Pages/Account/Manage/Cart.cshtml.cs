using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
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
using Abocar.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Abocar.Areas.Identity.Pages.Account.ExternalLoginModel;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class CartModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        
        public CartModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            cartItem = new List<CartItem>(); // Initialize the cartItem list
            civm = new cartItemViewModel();
        }

        [TempData]
        public string StatusMessage { get; set; }
        public List<CartItem> cartItem { get; set; } // Removed [BindProperty] attribute
        public cartItemViewModel civm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAsync();
            return Page();
        }

        public async Task LoadAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            TempData["Tel"] = user.PhoneNumber;
            var cartItems = await _context.CartItems
                                          .Where(x => x.UserId == user.Id)
                                          .ToListAsync();

            var productIds = cartItems.Select(x => x.ProductId).ToArray();
            var products = await _context.Products
                                         .Where(p => productIds.Contains(p.Id))
                                         .ToListAsync();
            var image = await _context.ProductImages
                                        .ToListAsync();
            var review = await _context.Review
                                        .Where(p => productIds.Contains(p.ProductId))
                                        .ToListAsync();
            var userAddress = await _context.Address
                                        .Where(p => p.UserId == user.Id)
                                        .FirstOrDefaultAsync();
            var deliveryOption = await _context.DeliveryOption
                                        .Where(x => x.isActive == true)
                                        .ToListAsync();
            var paymentOption = await _context.PaymentOption
                                        .Where(x => x.isActive == true)
                                        .ToListAsync();
            var pickUpLocation = await _context.LocalPickUp
                                        .Where(x => x.isActive == true)
                                        .ToListAsync();
            decimal subTotal = 0;
            foreach (var item in cartItems)
            {
                var prod = products.Where(x => x.Id == item.ProductId).FirstOrDefault();
                subTotal = subTotal + (item.Quantity * Convert.ToDecimal(prod.MainPrice));
            }
            TempData["Total"] = subTotal;
            ViewData["pickUpLocation"] = pickUpLocation;
            civm = new cartItemViewModel
            {
                Product = products,
                Image = image,
                Review = review,
                CartItem = cartItems,
                Address = userAddress,
                DeliveryOption = deliveryOption,
                PaymentOptions = paymentOption
            };
        }

        public IActionResult OnPost(int cartItemId, string quantityAction)
        {
            var cartItem = _context.CartItems.Where(x => x.Id == cartItemId).FirstOrDefault();
            if (cartItem == null)
            {
                return NotFound();
            }
            if (quantityAction == "increase")
            {
                cartItem.Quantity++;
            }
            else if (quantityAction == "decrease" && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            _context.CartItems.Update(cartItem);
            _context.SaveChanges();
            return RedirectToPage(); 
        }

        public IActionResult OnPostDelete(int cartItemId)
        {
            var cartItem = _context.CartItems.Where(x => x.Id == cartItemId).FirstOrDefault();
            if (cartItem == null)
            {
                return NotFound();
            }
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Product Removed successfully";
            return RedirectToPage();
        }

        /*public async Task<IActionResult> OnPostAddToWishlistAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            wishlist.User   Product = id;
            wishlist.UserId = user.Id;
            wishlist.CreatedAt = DateTime.Now;

            await _context.Wishlist.AddAsync(wishlist);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Item add successfully";
            return RedirectToPage("/Account/Manage/Cart");
        }
*/
    }

}
