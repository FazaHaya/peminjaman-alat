using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Pages.Petugas
{
    [Authorize(Roles = "Petugas,Admin")]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalMenunggu { get; set; }
        public int TotalDipinjam { get; set; }
        public int TotalKembaliHariIni { get; set; }

        public class PersetujuanView
        {
            public int IdPeminjaman { get; set; }
            public string NamaPeminjam { get; set; }
            public string NamaAlat { get; set; }
            public int Jumlah { get; set; }
            public DateTime? TanggalPinjam { get; set; }
            public string Status { get; set; }
        }

        public List<PersetujuanView> DaftarVerifikasi { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalMenunggu = await _context.Peminjamans
                .CountAsync(x =>
                    x.Status == "Menunggu" ||
                    x.Status == "Pending" ||
                    x.Status == "0");

            TotalDipinjam = await _context.Peminjamans
                .CountAsync(x =>
                    x.Status == "Disetujui" ||
                    x.Status == "Dipinjam" ||
                    x.Status == "1");

            TotalKembaliHariIni = await _context.Peminjamans
                .CountAsync(x =>
                    x.TanggalKembali.Date == DateTime.Today);

            DaftarVerifikasi = await
                (from p in _context.Peminjamans
                 join u in _context.Users
                    on p.IdUser equals u.IdUser
                 join d in _context.PeminjamanDetails
                    on p.IdPeminjaman equals d.IdPeminjaman
                 join a in _context.Alats
                    on d.IdAlat equals a.IdAlat
                 where p.Status == "Menunggu"
                    || p.Status == "Pending"
                    || p.Status == "0"
                 orderby p.TanggalPinjam descending
                 select new PersetujuanView
                 {
                     IdPeminjaman = p.IdPeminjaman,
                     NamaPeminjam = u.Nama,
                     NamaAlat = a.NamaAlat,
                     Jumlah = d.Jumlah,
                     TanggalPinjam = p.TanggalPinjam,
                     Status = p.Status
                 })
                 .Take(5)
                 .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var pinjam = await _context.Peminjamans
                .FirstOrDefaultAsync(x => x.IdPeminjaman == id);

            if (pinjam == null)
                return RedirectToPage();

            pinjam.Status = "Disetujui";

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var pinjam = await _context.Peminjamans
                .FirstOrDefaultAsync(x => x.IdPeminjaman == id);

            if (pinjam == null)
                return RedirectToPage();

            pinjam.Status = "Ditolak";

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}