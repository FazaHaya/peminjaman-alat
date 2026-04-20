using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Pages.Petugas
{
    [Authorize(Roles = "Petugas,Admin")]
    public class MonitoringModel : PageModel
    {
        private readonly AppDbContext _context;

        public MonitoringModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Peminjaman> DataMonitoring { get; set; } = new();

        public async Task OnGetAsync()
        {
            DataMonitoring = await _context.Peminjamans
                .Include(x => x.User)
                .OrderByDescending(x => x.TanggalPinjam)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostKonfirmasiTerimaAsync(
            int id,
            string kondisi,
            decimal biayaKerusakan)
        {
            var pinjam = await _context.Peminjamans
                .FirstOrDefaultAsync(x => x.IdPeminjaman == id);

            if (pinjam == null)
                return RedirectToPage();

            decimal dendaTelat = 0;

            if (DateTime.Now.Date > pinjam.TanggalKembali.Date)
            {
                int telat =
                    (DateTime.Now.Date - pinjam.TanggalKembali.Date).Days;

                dendaTelat = telat * 5000;
            }

            var pengembalian = new Pengembalian
            {
                IdPeminjaman = id,
                TanggalDikembalikan = DateTime.Now,
                KondisiKembali = kondisi
            };

            _context.Pengembalians.Add(pengembalian);
            await _context.SaveChangesAsync();

            decimal total = dendaTelat + biayaKerusakan;

            if (total > 0)
            {
                _context.Dendas.Add(new Denda
                {
                    id_pengembalian = pengembalian.IdPengembalian,
                    denda_terlambat = dendaTelat,
                    denda_kerusakan = biayaKerusakan,
                    total_denda = total,
                    status_pembayaran = "Belum Lunas"
                });

                pinjam.Status = "4";
            }
            else
            {
                pinjam.Status = "2";
            }

            var detail = await _context.PeminjamanDetails
                .Where(x => x.IdPeminjaman == id)
                .ToListAsync();

            foreach (var item in detail)
            {
                if (kondisi != "Hilang")
                {
                    var alat = await _context.Alats
                        .FirstOrDefaultAsync(x => x.IdAlat == item.IdAlat);

                    if (alat != null)
                        alat.Stok += item.Jumlah;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostBayarLunasAsync(int id)
        {
            var pinjam = await _context.Peminjamans
                .FirstOrDefaultAsync(x => x.IdPeminjaman == id);

            if (pinjam != null)
                pinjam.Status = "2";

            var denda = await (
                from d in _context.Dendas
                join pg in _context.Pengembalians
                    on d.id_pengembalian equals pg.IdPengembalian
                where pg.IdPeminjaman == id
                select d
            ).FirstOrDefaultAsync();

            if (denda != null)
            {
                denda.status_pembayaran = "Lunas";
                denda.tanggal_pembayaran = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}