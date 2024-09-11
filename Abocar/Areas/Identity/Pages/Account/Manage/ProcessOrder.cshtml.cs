using Abocar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class ProcessOrderModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ProcessOrderModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }

        public async Task<IActionResult> OnGetAsync(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return NotFound();
            }

            Order = await _context.Order
                .Include(o => o.OrderProduct)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (Order == null)
            {
                return NotFound();
            }

            ViewData["Order"] = Order;

            return Page();
        }
    }
}
