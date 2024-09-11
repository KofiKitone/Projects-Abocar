using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Cryptography;
using Newtonsoft.Json;
using SQLitePCL;
using System.Threading.Tasks;

namespace Abocar.Data
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("abocar.shops@gmail.com", "oltmqonzmvhvmcxa");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendContactEmailAsync(string userEmail, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(userEmail, userEmail)); // User's email as sender
            emailMessage.To.Add(new MailboxAddress(emailSettings["AdminName"], emailSettings["AdminEmail"])); // Admin's email
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("abocar.shops@gmail.com", "oltmqonzmvhvmcxa");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

    }


    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }



    public interface ICartWishlistService
    {
        Task<int> GetCartItemsCountAsync(string userId);
        Task<int> GetWishlistItemsCountAsync(string userId);
    }

    public class CartWishlistService : ICartWishlistService
    {
        private readonly ApplicationDbContext _context;

        public CartWishlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCartItemsCountAsync(string userId)
        {
            return await _context.CartItems.CountAsync(ci => ci.UserId == userId);
        }

        public async Task<int> GetWishlistItemsCountAsync(string userId)
        {
            return await _context.Wishlist.CountAsync(wi => wi.UserId == userId);
        }
    }





    public class CartWishlistViewComponent : ViewComponent
    {
        private readonly ICartWishlistService _cartWishlistService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartWishlistViewComponent(ICartWishlistService cartWishlistService, UserManager<ApplicationUser> userManager)
        {
            _cartWishlistService = cartWishlistService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var cartItemsCount = await _cartWishlistService.GetCartItemsCountAsync(user.Id);
                var wishlistItemsCount = await _cartWishlistService.GetWishlistItemsCountAsync(user.Id);

                var model = new CartWishlistViewModel
                {
                    CartItemsCount = cartItemsCount,
                    WishlistItemsCount = wishlistItemsCount
                };

                return View(model);
            }

            return View(new CartWishlistViewModel());
        }
    }




    public class CartWishlistViewModel
    {
        public int CartItemsCount { get; set; }
        public int WishlistItemsCount { get; set; }
    }






    // ViewComponents/CategoryDropdownViewComponent.cs
    public class CategoryDropdownViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryDropdownViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.SubCategories.ToListAsync();
            return View(categories);
        }
    }
}


