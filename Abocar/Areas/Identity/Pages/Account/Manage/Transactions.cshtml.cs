using Abocar.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class TransactionsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public TransactionsModel(UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            
        }
        public async Task OnGet()
        {
            var con = await _context.Order
                .Where(x => x.isPaid == true || x.PaymentOption == "Cash On Delivery")
                .Include(x => x.OrderProduct)
                .ToListAsync();

            if (con != null)
            {
                string ee = User.Identity?.Name;
                var transactions = new List<VendorTransactionModel>();

                var user = await _userManager.FindByNameAsync(ee);
                var roles = await _userManager.GetRolesAsync(user);

                bool isAdmin = roles.Contains("Administrator");

                List<Product> products;
                List<Vendor> vendors;
                List<Transaction> transactionsDb;

                if (isAdmin)
                {
                    // Fetch all necessary data for the administrator
                    products = await _context.Products.ToListAsync();
                    vendors = await _context.Vendors.ToListAsync();
                    transactionsDb = await _context.Transactions.ToListAsync();
                }
                else
                {
                    // Fetch only the current vendor
                    var vend = await _context.Vendors.Where(x => x.UserId == ee).FirstOrDefaultAsync();
                    if (vend == null)
                    {
                        // Handle case where vendor is not found
                        return;
                    }

                    products = await _context.Products.Where(p => p.VendorId == vend.Id).ToListAsync();
                    vendors = new List<Vendor> { vend };
                    transactionsDb = await _context.Transactions.ToListAsync();
                }

                foreach (var order in con)
                {
                    var customer = await _userManager.FindByIdAsync(order.UserId);

                    if (customer != null)
                    {
                        foreach (var orderProduct in order.OrderProduct)
                        {
                            var product = products.FirstOrDefault(p => p.Id == orderProduct.ProductId);
                            if (product == null) continue;

                            var vendor = vendors.FirstOrDefault(v => v.Id == product.VendorId);
                            if (vendor == null) continue;

                            var transaction = transactionsDb.FirstOrDefault(t => t.OrderId == order.OrderNumber);
                            if (transaction == null) continue;

                            var trans = new VendorTransactionModel
                            {
                                Vendor = vendor.UserId,
                                Email = customer.Email,
                                Name = customer.FirstName + " " + customer.LastName,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                Price = Convert.ToDecimal(product.MainPrice),
                                Quantity = orderProduct.Quantity,
                                CreatedAt = order.OrderDate,
                                Status = order.isPaid,
                                TransactionReference = transaction.TransactionReference,
                                OrderId = order.OrderNumber,
                                PaymentOption = order.PaymentOption,
                            };

                            transactions.Add(trans);
                        }
                    }
                }

                if (isAdmin)
                {
                    ViewData["Transactions"] = transactions.ToList();
                }
                else
                {
                    var vend = vendors.FirstOrDefault();
                    ViewData["Transactions"] = transactions.Where(x => x.Vendor == vend.UserId).ToList();
                }
            }
        }



        public async Task LoadAsync ()
        {
            var order = await _context.Order.Where(x => x.isPaid == true).ToListAsync();
            var transactions = new List<VendorTransactionModel>();
            foreach (var item in order)
            {
                ApplicationUser itemUser = await _userManager.FindByIdAsync(item.UserId);
                if (item is not null)
                {
                    foreach (var item2 in item.OrderProduct)
                    {
                        var product = await _context.Products.Where(x => x.Id == item2.Id).FirstOrDefaultAsync();
                        var vendor = await _context.Vendors.Where(x => x.Id == product.VendorId).FirstOrDefaultAsync();
                        var transaction = await _context.Transactions.Where(x => x.OrderId == item.OrderNumber).FirstOrDefaultAsync();
                        var trans = new VendorTransactionModel
                        {
                            Vendor = vendor.UserId,
                            Email = itemUser.Email,
                            Name = itemUser.FirstName + " " + itemUser.LastName,
                            ProductId = product.Id,
                            ProductName = product.Name,
                            Price = Convert.ToDecimal(product.MainPrice),
                            Quantity = item2.Quantity,
                            CreatedAt = item.OrderDate,
                            Status = item.isPaid,
                            TransactionReference = transaction.TransactionReference,
                            OrderId = item.OrderNumber,
                            PaymentOption = item.PaymentOption,
                        };
                        transactions.Add(trans);
                    }
                    ViewData["Transactions"] = transactions;
                }
            }
        }
    }
}
