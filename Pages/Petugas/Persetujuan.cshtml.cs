using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Pages.Petugas
{
    public class PersetujuanModel : PageModel
    {
        private readonly AppDbContext _context;
        public PersetujuanModel(AppDbContext context) => _context = context;
        public class PersetujuanView
        {
            public int IdPeminjaman { get; set; }
            public string? NamaPeminjam { get; set; }
            public string? NamaAlat { get; set; }
            public int? Jumlah { get; set; }
            public DateTime? TanggalPinjam { get; set; }
            public string? Status { get; set; }
        }
        public List<PersetujuanView> DaftarVerifikasi { get; set; } = new();

        public async Task OnGetAsync()
        {
            DaftarVerifikasi = await (
                    from p in _context.Peminjamans
                    join u in _context.Users on p.IdUser equals u.IdUser
                    join d in _context.PeminjamanDetails on p.IdPeminjaman equals d.IdPeminjaman
                    join a in _context.Alats on d.IdAlat equals a.IdAlat
                    where p.Status == "0" || p.Status == "Menunggu Approval"
                    select new PersetujuanView
                    {
                        IdPeminjaman = p.IdPeminjaman,
                        NamaPeminjam = u.Nama ?? "-",
                        NamaAlat = a.NamaAlat ?? "-",
                        Jumlah = d.Jumlah,
                        TanggalPinjam = p.TanggalPinjam,
                        Status = p.Status ?? "0"
                    }).ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var pinjaman = await _context.Peminjamans
                .FirstOrDefaultAsync(x => x.IdPeminjaman == id);

            if (pinjaman == null)
                return RedirectToPage();

            var details = await _context.PeminjamanDetails
                .Where(x => x.IdPeminjaman == id)
                .ToListAsync();

            foreach (var item in details)
            {
                var alat = await _context.Alats.FindAsync(item.IdAlat);

                if (alat != null)
                {
                    if (alat.Stok < item.Jumlah)
                    {
                        TempData["Error"] = $"Stok {alat.NamaAlat} tidak cukup!";
                        return RedirectToPage();
                    }

                    alat.Stok -= item.Jumlah;
                }
            }

            pinjaman.Status = "1"; // dipinjam

            await _context.SaveChangesAsync();

            TempData["Success"] = "Peminjaman disetujui.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var pinjaman = await _context.Peminjamans
                .FirstOrDefaultAsync(x => x.IdPeminjaman == id);

            if (pinjaman == null)
                return RedirectToPage();

            pinjaman.Status = "Ditolak";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Peminjaman ditolak.";
            return RedirectToPage();
        }
    }
}