using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Abocar.Models; // Make sure this namespace includes ApplicationDbContext and model classes

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class OptionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public OptionsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            var deliveryOptions = await _context.DeliveryOption.ToListAsync();
            if (deliveryOptions != null)
            {
                ViewData["DeliveryOptions"] = deliveryOptions;
            }

            var paymentOptions = await _context.PaymentOption.ToListAsync();
            if (paymentOptions != null)
            {
                ViewData["PaymentOptions"] = paymentOptions;
            }

            var localPickPoints = await _context.LocalPickUp.ToListAsync();
            if (localPickPoints != null)
            {
                ViewData["LocalPickPoints"] = localPickPoints;
            }
            var subCategories = await _context.SubCategories.ToListAsync();
            if (subCategories != null)
            {
                ViewData["SubCategories"] = subCategories;
            }
            var brand = await _context.Brand.ToListAsync();
            if (brand != null)
            {
                ViewData["Brands"] = brand;
            }
        }

        public async Task<IActionResult> OnGetDeleteOptionAsync(int id, string data)
        {
            await FindAndDelete(id, data);
            return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
        }

        private async Task FindAndDelete(int id, string data)
        {
            if (id != 0 && data != null)
            {
                if (data == "LocalPickUp")
                {
                    var item = await _context.LocalPickUp.FindAsync(id);
                    if (item != null)
                    {
                        _context.LocalPickUp.Remove(item);
                        TempData["SuccessMessage"] = "Item removed successfully";
                        await _context.SaveChangesAsync();
                    }
                }
                else if (data == "Delivery")
                {
                    var item = await _context.DeliveryOption.FindAsync(id);
                    if (item != null)
                    {
                        _context.DeliveryOption.Remove(item);
                        TempData["SuccessMessage"] = "Item removed successfully";
                        await _context.SaveChangesAsync();
                    }
                }
                else if (data == "Payment")
                {
                    var item = await _context.PaymentOption.FindAsync(id);
                    if (item != null)
                    {
                        _context.PaymentOption.Remove(item);
                        TempData["SuccessMessage"] = "Item removed successfully";
                        await _context.SaveChangesAsync();
                    }
                }
                else if (data == "SubCategories")
                {
                    var item = await _context.SubCategories.FindAsync(id);
                    if (item != null)
                    {
                        _context.SubCategories.Remove(item);
                        TempData["SuccessMessage"] = "Item removed successfully";
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
