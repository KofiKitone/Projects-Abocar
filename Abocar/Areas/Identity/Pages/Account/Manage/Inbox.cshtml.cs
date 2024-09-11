using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abocar.Data;
using Abocar.Models;
using Microsoft.EntityFrameworkCore;
using Abocar.Controllers;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class InboxModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InboxModel(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public IList<Message> Messages { get; set; }

        public async Task LoadAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            if (user != null)
            {
                Messages = await _context.Messages
                .Where(m => m.UserId == user.Id)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
            }
            
            var messageViewModel = new List<MessageViewModel>();
            foreach (var message in Messages)
            {
                if (message.OrderId != null || message.OrderId != 0)
                {
                    var combProducts = new List<ProductAndImage>();
                    var order = _context.Order.Where(x => x.Id == message.OrderId).Include(x => x.OrderProduct).FirstOrDefault();
                    if (order != null)
                    {
                        var orderProducts = order.OrderProduct.ToList();
                        foreach (var products in orderProducts)
                        {
                            var product = _context.Products.Where(x => x.Id == products.ProductId).FirstOrDefault();
                            var image = _context.ProductImages.Where(x => x.ProductId == products.ProductId).FirstOrDefault();
                            if (product != null || image  != null)
                            {
                                var pi = new ProductAndImage
                                {
                                    Product = product,
                                    Image = image
                                };
                                combProducts.Add(pi);
                            }
                            
                        }
                    }
                    var mVModel = new MessageViewModel
                    {
                        Message = message,
                        Product = combProducts,
                    };
                    messageViewModel.Add(mVModel);
                }else
                {
                    var mvModel = new MessageViewModel
                    {
                        Message = message,
                    };
                    messageViewModel.Add(mvModel);
                }
                
            }
            ViewData["Messages"] = messageViewModel;

        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAsync();
            return Page();
        }

        // Endpoint to fetch message details
        public async Task<IActionResult> OnGetMessageDetailsAsync(int messageId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
            {
                return NotFound();
            }

            return Partial("_viewMessagePartial", message);
        }

    }
}
