using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Pages.Petugas
{
    [Authorize(Roles = "Petugas,Admin")]
    public class LaporanModel : PageModel
    {
        private readonly AppDbContext _context;

        public LaporanModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Peminjaman> LaporanData { get; set; } = new();

        public int TotalTransaksi { get; set; }
        public int TotalAlatKeluar { get; set; }

        public async Task OnGetAsync(
            string status,
            DateTime? mulai,
            DateTime? selesai)
        {
            var query = _context.Peminjamans
                .Include(x => x.User)
                .AsQueryable();

            // default tampil final saja
            if (string.IsNullOrEmpty(status))
            {
                query = query.Where(x =>
                    x.Status == "2" ||
                    x.Status == "Selesai" ||
                    x.Status == "Ditolak");
            }
            else if (status == "2")
            {
                query = query.Where(x =>
                    x.Status == "2" ||
                    x.Status == "Selesai");
            }
            else
            {
                query = query.Where(x => x.Status == status);
            }

            if (mulai.HasValue)
                query = query.Where(x =>
                    x.TanggalPinjam.Date >= mulai.Value.Date);

            if (selesai.HasValue)
                query = query.Where(x =>
                    x.TanggalPinjam.Date <= selesai.Value.Date);

            LaporanData = await query
                .OrderByDescending(x => x.TanggalPinjam)
                .ToListAsync();

            TotalTransaksi = LaporanData.Count;

            var ids = LaporanData
                .Select(x => x.IdPeminjaman)
                .ToList();

            TotalAlatKeluar = await _context.PeminjamanDetails
                .Where(x => ids.Contains(x.IdPeminjaman))
                .SumAsync(x => x.Jumlah);
        }
    }
}