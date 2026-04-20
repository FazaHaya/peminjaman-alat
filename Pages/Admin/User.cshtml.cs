using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserModel : PageModel
    {
        private readonly AppDbContext _context;

        public UserModel(AppDbContext context)
        {
            _context = context;
        }

        public List<User> DataUser { get; set; } = new();

        [BindProperty]
        public User Input { get; set; }

        [BindProperty]
        public int IdUser { get; set; }

        public void OnGet()
        {
            DataUser = _context.Users
                .OrderBy(x => x.Nama)
                .ToList();
        }

        public IActionResult OnPostCreate()
        {
            if (string.IsNullOrWhiteSpace(Input.Nama) ||
                string.IsNullOrWhiteSpace(Input.Username) ||
                string.IsNullOrWhiteSpace(Input.Password) ||
                string.IsNullOrWhiteSpace(Input.Role))
            {
                return RedirectToPage();
            }

            bool exist = _context.Users
                .Any(x => x.Username == Input.Username);

            if (exist)
                return RedirectToPage();

            Input.CreatedAt = DateTime.Now;
            Input.Password = BCrypt.Net.BCrypt.HashPassword(Input.Password);

            _context.Users.Add(Input);
            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var user = _context.Users
                .FirstOrDefault(x => x.IdUser == Input.IdUser);

            if (user == null)
                return RedirectToPage();

            bool usernameDipakai = _context.Users.Any(x =>
                x.Username == Input.Username &&
                x.IdUser != Input.IdUser);

            if (usernameDipakai)
                return RedirectToPage();

            user.Nama = Input.Nama;
            user.Username = Input.Username;
            user.Role = Input.Role;

            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                user.Password =
                    BCrypt.Net.BCrypt.HashPassword(Input.Password);
            }

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            var user = _context.Users
                .FirstOrDefault(x => x.IdUser == IdUser);

            if (user == null)
                return RedirectToPage();

            if (User.Identity?.Name == user.Username)
                return RedirectToPage();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToPage();
        }
    }
}