using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.API.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public string? Msg { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // cek username sudah ada
            bool exist = _context.Users
                .Any(x => x.Username == Input.Username);

            if (exist)
            {
                Msg = "Username sudah digunakan.";
                return Page();
            }

            // validasi password minimal
            if (Input.Password.Length < 5)
            {
                Msg = "Password minimal 5 karakter.";
                return Page();
            }

            var user = new User
            {
                Nama = Input.Nama,
                Username = Input.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(Input.Password),
                Role = "Peminjam",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Registrasi berhasil, silakan login.";

            return RedirectToPage("/Auth/Login");
        }

        public class RegisterInput
        {
            public string Nama { get; set; } = "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}