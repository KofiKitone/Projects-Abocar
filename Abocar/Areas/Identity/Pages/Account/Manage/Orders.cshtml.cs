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
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class OrdersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public OrdersModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            order = new List<Order>();
        }

        public List<Order> order { get; set; }

        private async Task LoadAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            order = await _context.Order
                .Where(x => x.UserId == user.Id)
                .Include(x => x.OrderProduct)
                .ToListAsync();
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Address address = await _context.Address.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
            await LoadAsync();
            return Page();
        }
    }
}
