using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Abocar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Net;

namespace Abocar.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            var message = GenerateEmailBody(Input.Email, HtmlEncoder.Default.Encode(callbackUrl));
            await _emailSender.SendEmailAsync(Input.Email, "Email Verification", message);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }


        public static string GenerateEmailBody(string email, string link)
        {
            return $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2E4053;'>Account Verification</h2>
                <p>Dear {email},</p>
                <p>To complete your registration, please verify your email address by clicking the link below:</p>
                <p><a href='{link}' style='color: #3498db;'>Verify Email</a></p>
                <p>If the above link doesn't work, copy and paste the following URL into your web browser:</p>
                <p>{link}</p>
                <p>If you did not create an account, no further action is required.</p>
                <p>Best regards,</p>
                <p>Your Company Name</p>
            </div>";
        }

        private void SendEmailAsync(string email, string Subject, string MailMessage)
		{
			string myEmail = "prospaaddico@gmail.com";
			string myPassword = "rlppgcsyawfrldsu";
			string smtpAddress = "smtp.gmail.com";

			MailMessage message = new MailMessage();
			message.From = new MailAddress(myEmail);
			message.Subject = Subject;
			message.To.Add(new MailAddress(Input.Email));
			message.Body = MailMessage;
			message.IsBodyHtml = true;

			var smptClient = new SmtpClient(smtpAddress)
			{
				Port = 587,
				Credentials = new NetworkCredential(myEmail, myPassword),
				EnableSsl = true,
			};
			smptClient.Send(message);
		}
	}
}
