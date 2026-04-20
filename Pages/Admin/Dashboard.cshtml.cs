using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using PeminjamanAlat.Models;
using PeminjamanAlat.Data;

namespace PeminjamanAlat.API.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalUsers { get; set; }
        public int TotalAlat { get; set; }
        public int TotalPeminjaman { get; set; }
        public int TotalPending { get; set; }

        public void OnGet()
        {
            // total user
            TotalUsers = _context.Users.Count();

            // total alat
            TotalAlat = _context.Alats.Count();

            // total peminjaman
            TotalPeminjaman = _context.Peminjamans.Count();

            // total pending
            TotalPending = _context.Peminjamans
                .Count(x => x.Status == "Pending");

            // simpan nama ke session jika belum ada
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                HttpContext.Session.SetString(
                    "UserName",
                    User.Identity?.Name ?? "Administrator"
                );
            }
        }
    }
}