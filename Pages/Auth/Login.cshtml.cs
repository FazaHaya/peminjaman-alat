using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeminjamanAlat.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

public class LoginModel : PageModel
{
    private readonly AppDbContext _context;

    public LoginModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string Msg { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Username == Username);

        if (user == null)
        {
            Msg = "Username tidak ditemukan";
            return Page();
        }

        bool valid = BCrypt.Net.BCrypt.Verify(
            Password,
            user.Password
        );

        if (!valid)
        {
            Msg = "Password salah";
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,user.Nama),
            new Claim(ClaimTypes.Role,user.Role),
            new Claim(ClaimTypes.NameIdentifier,user.IdUser.ToString())
        };

        var identity = new ClaimsIdentity(
            claims, "CookieAuth");

        await HttpContext.SignInAsync(
            "CookieAuth",
            new ClaimsPrincipal(identity));

        if (user.Role == "Admin")
            return RedirectToPage("/Admin/Dashboard");

        if (user.Role == "Petugas")
            return RedirectToPage("/Petugas/Dashboard");

        return RedirectToPage("/Peminjam/Dashboard");
    }
}