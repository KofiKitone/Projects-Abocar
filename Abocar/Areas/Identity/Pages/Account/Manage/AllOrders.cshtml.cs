using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class AllOrdersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AllOrdersModel(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        List<AllOrdersViewModel> orders = new List<AllOrdersViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var allOrder = await _context.Order.ToListAsync();
            foreach (var order in allOrder)
            {
                ApplicationUser user = _context.Users.Where(x=>x.Id == order.UserId).FirstOrDefault();
                if (user != null)
                {
                    var ord = new AllOrdersViewModel
                    {
                        OrderNumber = order.OrderNumber,
                        CustomerEmail = user.Email,
                        Date = order.OrderDate,
                        Total = order.Total,
                        Status = order.OrderStatus,
                        isPaid = order.isPaid,
                    };
                    
                    orders.Add(ord);
                }
                
            }
            ViewData["Orders"] = orders;
            return Page();
        }
    }
}
