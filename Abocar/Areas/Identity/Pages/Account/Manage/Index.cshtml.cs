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
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Profile Image")]
            public string? ImagePath { get; set; }
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            public string Nationality { get; set; }
            [NotMapped]
            public IFormFile? ImageFile { get; set; }

        }

        private async Task LoadAsync(ApplicationUser user)
        {
            /* var userName = await _userManager.GetUserNameAsync(user);
             var phoneNumber = await _userManager.GetPhoneNumberAsync(user);*/

            ApplicationUser use = await _userManager.GetUserAsync(User);
   
            Username = use.UserName;

            Input = new InputModel
            {
                PhoneNumber = use.PhoneNumber,
                ImagePath = use.ImagePath,
                FirstName = use.FirstName,
                LastName = use.LastName,
                Nationality = use.Nationaltiy
            };
        }

        public async Task<string> SaveImage(IFormFile imgFile, ApplicationUser user)
        {
            if (imgFile == null || imgFile.Length == 0)
            {
                throw new ArgumentException("No image file uploaded.");
            }

            // Validate file size (optional)
            if (imgFile.Length > 5242880) // 1 MB limit (adjust as needed)
            {
                throw new ArgumentException("Image file size exceeds the limit.");
            }

            // Validate file type (optional)
            string validExtensions = ".jpg,.jpeg,.png"; // Allowed extensions
            if (!validExtensions.Contains(Path.GetExtension(imgFile.FileName).ToLower()))
            {
                throw new ArgumentException("Invalid image file type.");
            }

            // Generate unique filename
            string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName);
            string extension = Path.GetExtension(imgFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

            // Construct image path (use a separate variable)
            string savedImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProfilePictures", fileName);

            // Save the file with error handling
            try
            {
                using (var fileStream = new FileStream(savedImagePath, FileMode.Create))
                {
                    await imgFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving image file: " + ex.Message);
            }

            // Update user's image path and return the saved path
            user.ImagePath = "profilepictures/" + fileName;
            await _userManager.UpdateAsync(user);
            return savedImagePath; // Return the actual saved path for potential use
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(InputModel Input)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            else
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                user.LastName = Input.LastName;
                user.FirstName = Input.FirstName;
                user.Nationaltiy = Input.Nationality;
                string savedImagePath = await SaveImage(Input.ImageFile, user);
                var saving = await _userManager.UpdateAsync(user);
                if (!saving.Succeeded)
                {
                    TempData["ErrorMessage"] = "Unexpected error when trying to make changes 😢";
                    return RedirectToPage();
                }
            }
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Profile updated successfully 👍";
            return RedirectToPage();
        }
            
    }
}
