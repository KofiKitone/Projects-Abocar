using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abocar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AddressModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            public string AddressLine { get; set; }
            public string StreetNumber { get; set; }
            public string PostalCods { get; set; }
            public string DigitalAddress { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
        }

        private async Task LoadAsync(Address address)
        {

            ApplicationUser user = await _userManager.GetUserAsync(User);
            address = await _context.Address.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

            Input = new InputModel
            {
                AddressLine = address.AdressLine,
                StreetNumber = address.StreetNumber,
                PostalCods = address.PostalCods,
                DigitalAddress = address.DigitalAddress,
                City = address.City,
                Region = address.Region,
            };
        }

        public async Task<IActionResult> OnGetAsync( string data)
        {
            Console.WriteLine("------------------------------------------- we are here -------------------------------------------");
            if (data == "FromCheckOut")
            {
                TempData["RouteBackToCart"] = "Yes";
            }
            string email = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Address address = await _context.Address.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
            if (address == null) { return Page(); }
            else
            {
                await LoadAsync(address);
                return Page();
            }
        }


        public async Task<IActionResult> OnPostAsync(string data)
        {
            Console.WriteLine("------------------------------------------The Post method of Address was called");

            string email = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Console.WriteLine("User is not null " + user.Email);
            Address addre = await _context.Address.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
            if (addre == null)
            {
                Console.WriteLine("Address is null ");
                Address address = new Address
                {
                    UserId = user.Id,
                    AdressLine = Input.AddressLine,
                    StreetNumber = Input.StreetNumber,
                    PostalCods = Input.PostalCods,
                    DigitalAddress = Input.DigitalAddress,
                    City = Input.City,
                    Region = Input.Region,
                    Country = user.Nationaltiy
                };

                await _context.Address.AddAsync(address);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Address add successfully 👍";
                if (data == "FromCart")
                {
                    TempData["RouteBackToCart"] = "No";
                    return RedirectToPage("/Account/Manage/Cart", new { area = "Identity" });
                }
                else
                {
                    return RedirectToPage();
                }
            }
            else
            {
                Console.WriteLine("----------------------------------------------Address is not null");
                addre.AdressLine = Input.AddressLine;
                addre.StreetNumber = Input.StreetNumber;
                addre.PostalCods = Input.PostalCods;
                addre.DigitalAddress = Input.DigitalAddress;
                addre.City = Input.City;
                addre.Region = Input.Region;
                addre.Country = user.Nationaltiy;
                addre.UserId = user.Id;
                _context.Address.Update(addre);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Address updated successfully 👍";
                if (data == "FromCart")
                {
                    TempData["RouteBackToCart"] = "No";
                    return RedirectToPage("/Account/Manage/Cart", new { area = "Identity" });
                }
                else
                {
                    return RedirectToPage();
                }

            }
        }
    }
}
