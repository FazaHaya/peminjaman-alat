using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PeminjamanAlat.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await LogoutUser();
            return RedirectToPage("/Auth/Login");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LogoutUser();
            return RedirectToPage("/Auth/Login");
        }

        private async Task LogoutUser()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();
        }
    }
}