using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Abocar.Areas.Identity.Pages.Account.Manage
{
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersModel (UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Administrator")]
        public void OnGet()
        {
            var theList = new List<userViewModel>();
            var allUsers = _userManager.Users.ToList();
            var allRoles = _roleManager.Roles.ToList();
            var allUserRoles = _context.UserRoles.ToList();
            var allVendors = _context.Vendors.ToList();

            foreach (var user in allUsers)
            {
                int postCount = 0;
                var userRoles = allUserRoles.Where(x => x.UserId == user.Id).Select(a => a.RoleId).FirstOrDefault();
                var roles = allRoles.Where(x => x.Id == userRoles).FirstOrDefault();
                var vendor = allVendors.Where(x => x.UserId == user.Email).FirstOrDefault();
                if (vendor != null)
                {
                    var product = _context.Products.Where(x => x.VendorId == vendor.Id).ToList();
                    postCount = product.Count();
                }
                var list = new userViewModel
                {
                    user = user,
                    userRoles = userRoles,
                    roles = roles,
                    posts = postCount,
                };
                theList.Add(list);
            }
            ViewData["AllUsers"] = theList;
            ViewData["Users"] = allUsers;
            ViewData["Roles"] = allRoles;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> OnPostAddAsync(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                await _userManager.AddToRoleAsync(user, role);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
