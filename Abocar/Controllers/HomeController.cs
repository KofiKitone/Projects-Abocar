using Abocar.Data;
using Abocar.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MimeKit;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using MailKit.Net.Smtp;


namespace Abocar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public int count = 1;
        public HomeController(ApplicationDbContext context,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            configuration = configuration;
            _emailSender = emailSender;
            _context = context;
        }
        
        public IActionResult Index(int min, int max, string? searchString, string? where, int CategoryId, string title)
        {
            if (title != null) { TempData["Title"] = title; }
            ViewData["Categories"] = _context.SubCategories.ToList();
            IQueryable<Product> productsQuery = _context.Products.Where(x => x.isActive);
            if (count > 1) { TempData["Second"] = "Yes"; } else { TempData["Second"] = "No"; }

            if (!String.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(x => x.Name.Contains(searchString));
                TempData["SearchString"] = searchString;

                if (!productsQuery.Any())
                {
                    TempData["SearchIsEmpty"] = "Yes";
                }
                else
                {
                    TempData["SearchIsEmpty"] = "No";
                    HttpContext.Session.SetObject("searchedProduct", productsQuery.ToList());
                }

            }

            if (min != 0 && max != 0)
            {
                if (HttpContext.Session.GetObject<List<Product>>("searchedProduct") != null)
                {
                    var storedProducts = HttpContext.Session.GetObject<List<Product>>("searchedProduct");
                    productsQuery = storedProducts.AsQueryable();

                    productsQuery = productsQuery.Where(x => Convert.ToDecimal(x.MainPrice) >= min && Convert.ToDecimal(x.MainPrice) <= max);

                    if (!productsQuery.Any())
                    {
                        TempData["PriceFilterEmpty"] = "Yes";
                    }
                    else
                    {
                        TempData["PriceFilterEmpty"] = "No";
                        TempData["FilterMode"] = "Yes";
                    }
                }
                else
                {
                    productsQuery = productsQuery.Where(x => Convert.ToDecimal(x.MainPrice) >= min && Convert.ToDecimal(x.MainPrice) <= max);

                    if (!productsQuery.Any())
                    {
                        TempData["PriceFilterEmpty"] = "Yes";
                    }
                    else
                    {
                        TempData["PriceFilterEmpty"] = "No";
                        TempData["FilterMode"] = "Yes";
                    }
                }
            }

            if (where == "CategorySearch" && CategoryId != 0)
            {
                productsQuery = productsQuery.Where(x => x.SubCategoryId == CategoryId);
                HttpContext.Session.SetObject("searchedProduct", productsQuery.ToList());
            }

            ProductViewModel product = new ProductViewModel
            {
                Product = productsQuery.ToList(),
                Image = _context.ProductImages.ToList(),
                Review = _context.Review.ToList()
            };
            count++;
            return View(product);
        }

        public IActionResult HomePage(int min, int max, string? NavsearchString, string? searchString, string? where, int CategoryId)
        {
            ViewData["Categories"] = _context.SubCategories.ToList();
            IQueryable<Product> productsQuery = _context.Products.Where(x => x.isActive);
            if (count > 1) { TempData["Second"] = "Yes"; } else { TempData["Second"] = "No"; }

            if (NavsearchString != null)
            {
                productsQuery = productsQuery.Where(x => x.Brand == NavsearchString);
                TempData["SearchString"] = NavsearchString;
            }

            if (searchString == "All")
            {
                productsQuery = productsQuery;
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(x => x.Name.Contains(searchString));
                TempData["SearchString"] = searchString;

                if (!productsQuery.Any())
                {
                    TempData["SearchIsEmpty"] = "Yes";
                }
                else
                {
                    TempData["SearchIsEmpty"] = "No";
                    HttpContext.Session.SetObject("searchedProduct", productsQuery.ToList());
                }
            }

            if (min != 0 && max != 0)
            {
                if (HttpContext.Session.GetObject<List<Product>>("searchedProduct") != null)
                {
                    var storedProducts = HttpContext.Session.GetObject<List<Product>>("searchedProduct");
                    productsQuery = storedProducts.AsQueryable();

                    productsQuery = productsQuery.Where(x => Convert.ToDecimal(x.MainPrice) >= min && Convert.ToDecimal(x.MainPrice) <= max);

                    if (!productsQuery.Any())
                    {
                        TempData["PriceFilterEmpty"] = "Yes";
                    }
                    else
                    {
                        TempData["PriceFilterEmpty"] = "No";
                    }
                }
                else
                {
                    decimal minPrice = Convert.ToDecimal(min);
                    decimal maxPrice = Convert.ToDecimal(max);
                    productsQuery = productsQuery.Where(x => x.Price >= minPrice && x.Price <= maxPrice);

                    if (!productsQuery.Any())
                    {
                        TempData["PriceFilterEmpty"] = "Yes";
                    }
                    else
                    {
                        TempData["PriceFilterEmpty"] = "No";
                        TempData["FilterMode"] = "Yes";
                    }
                }
            }

            if (where == "CategorySearch" || CategoryId != 0)
            {
                productsQuery = productsQuery.Where(x => x.SubCategoryId == CategoryId);
                HttpContext.Session.SetObject("searchedProduct", productsQuery.ToList());
            } 

            ProductViewModel product = new ProductViewModel
            {
                Product = productsQuery.ToList(),
                Image = _context.ProductImages.ToList(),
                Review = _context.Review.ToList()
            };
            count++;
            return View(product);
        }


        public void Filter(int min, int max)
        {
            var products = _context.Products.ToList();
            if (min != 0 && max != 0)
            {
                products = products.Where(x => x.Price >= min && x.Price <= max).ToList();
            }
            if (products.Count == 0) { TempData["PriceFilterEmpty"] = "Yes"; }
            ViewData["Prods"] = products;
            ViewData["Img"] = _context.ProductImages.ToList();
            ViewData["Rev"] = _context.Review.ToList();
            TempData["FilterMode"] = "Yes";
        }


        public IActionResult Privacy(string data)        
        {
            if (data != null)
            {
                if (data == "About")
                {
                    TempData["about"] = "Yes";
                }
                else if (data == "Privacy")
                {
                    TempData["privacy"] = "Yes";
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> sendMail(string Name, string Email, string Message)
        {
            await SendContactEmailAsync(Email, "Help Contact", Message);
            return RedirectToAction("HomePage", "Home");
        }

        public async Task SendContactEmailAsync(string userEmail, string subject, string message)
        {

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(userEmail, userEmail)); // User's email as sender
            emailMessage.To.Add(new MailboxAddress("Abocar", "abocar.shops@gmail.com")); // Admin's email
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("abocar.shops@gmail.com", "oltmqonzmvhvmcxa");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }




    }
}
