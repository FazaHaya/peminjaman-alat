using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using System.Security.Claims;

namespace PeminjamanAlat.Pages.Peminjam
{
    public class PinjamanModel : PageModel
    {
        private readonly AppDbContext _context;

        public PinjamanModel(AppDbContext context)
        {
            _context = context;
        }

        public class PinjamanView
        {
            public int IdPeminjaman { get; set; }
            public string? NamaAlat { get; set; }
            public DateTime? TanggalPinjam { get; set; }
            public string Status { get; set; } = "0";

            public decimal TotalDenda { get; set; }
            public string StatusBayar { get; set; } = "Lunas";
        }

        public List<PinjamanView> MyLoans { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userIdStr =
                HttpContext.Session.GetString("UserId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out int userId))
                return;

            MyLoans = await (
                from p in _context.Peminjamans
                where p.IdUser == userId

                join d in _context.PeminjamanDetails
                    on p.IdPeminjaman equals d.IdPeminjaman

                join a in _context.Alats
                    on d.IdAlat equals a.IdAlat

                join pg in _context.Pengembalians
                    on p.IdPeminjaman equals pg.IdPeminjaman
                    into pgJoin
                from pg in pgJoin.DefaultIfEmpty()

                join dn in _context.Dendas
                    on (pg != null ? pg.IdPengembalian : 0)
                    equals dn.id_pengembalian
                    into dnJoin
                from dn in dnJoin.DefaultIfEmpty()

                orderby p.IdPeminjaman descending

                select new PinjamanView
                {
                    IdPeminjaman = p.IdPeminjaman,
                    NamaAlat = a.NamaAlat,
                    TanggalPinjam = p.TanggalPinjam,

                    // PAKAI STATUS ASLI DB
                    Status = p.Status ?? "0",

                    TotalDenda = dn != null
                        ? dn.total_denda
                        : 0,

                    StatusBayar = dn != null
                        ? dn.status_pembayaran!
                        : "Lunas"
                }

            ).ToListAsync();
        }

        public async Task<IActionResult> OnPostAjukanKembaliAsync(int id)
        {
            var pinjam = await _context.Peminjamans
                .FirstOrDefaultAsync(x => x.IdPeminjaman == id);

            if (pinjam == null)
                return RedirectToPage();

            pinjam.Status = "3";
            pinjam.Catatan = "Menunggu verifikasi pengembalian";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Pengajuan berhasil dikirim ke petugas.";

            return RedirectToPage();
        }
    }
}