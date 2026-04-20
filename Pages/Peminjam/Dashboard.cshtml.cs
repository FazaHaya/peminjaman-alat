using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;

namespace PeminjamanAlat.Pages.Peminjam
{
    [Authorize(Roles = "Peminjam")]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalAktif { get; set; }
        public int TotalPending { get; set; }
        public int TotalRiwayat { get; set; }

        public async Task OnGetAsync()
        {
            var username = User.Identity.Name;

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == username);

            if (user == null) return;

            TotalAktif = await _context.Peminjamans
                .CountAsync(x =>
                    x.IdUser == user.IdUser &&
                    (x.Status == "1" || x.Status == "Dipinjam"));

            TotalPending = await _context.Peminjamans
                .CountAsync(x =>
                    x.IdUser == user.IdUser &&
                    (x.Status == "0" || x.Status == "Menunggu"));

            TotalRiwayat = await _context.Peminjamans
                .CountAsync(x => x.IdUser == user.IdUser);
        }
    }
}