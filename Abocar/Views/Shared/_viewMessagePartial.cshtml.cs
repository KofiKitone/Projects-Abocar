using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Abocar.Views.Shared
{
    public class _viewMessagePartialModel : PageModel
    {

        public IActionResult OnGet(int Id)
        {
            return Page();
        }

    }
}
