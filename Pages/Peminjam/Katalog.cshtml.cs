using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;
using System.Text.Json;

namespace PeminjamanAlat.Pages.Peminjam
{
    public class KatalogModel : PageModel
    {
        private readonly AppDbContext _context;
        public KatalogModel(AppDbContext context) => _context = context;

        public List<Alat> DaftarAlat { get; set; } = new();
        public string NamaSiswa { get; set; }

        [BindProperty]
        public DateTime TglPinjam { get; set; } = DateTime.Now;
        [BindProperty]
        public DateTime TglKembali { get; set; } = DateTime.Now.AddDays(3);
        [BindProperty]
        public string SelectedAlatJson { get; set; }

        public async Task OnGetAsync()
        {
            NamaSiswa = HttpContext.Session.GetString("UserName") ?? "Siswa";
            DaftarAlat = await _context.Alats.Where(a => a.Stok > 0).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr =
                HttpContext.Session.GetString("UserId")
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; 
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Auth/Login");

            if (string.IsNullOrEmpty(SelectedAlatJson) || SelectedAlatJson == "[]")
            {
                TempData["Error"] = "Keranjang masih kosong!";
                return RedirectToPage();
            }

            var peminjaman = new Peminjaman
            {
                IdUser = int.Parse(userIdStr),
                TanggalPinjam = TglPinjam,
                TanggalKembali = TglKembali,
                Status = "0"
            };

            _context.Peminjamans.Add(peminjaman);
            await _context.SaveChangesAsync();

            var idAlatList = JsonSerializer.Deserialize<List<int>>(SelectedAlatJson);
            foreach (var idAlat in idAlatList)
            {
                var alat = await _context.Alats.FindAsync(idAlat);

                if (alat != null && alat.Stok > 0)
                {
                    var detail = new PeminjamanDetail
                    {
                        IdPeminjaman = peminjaman.IdPeminjaman,
                        IdAlat = idAlat,
                        Jumlah = 1
                    };

                    _context.PeminjamanDetails.Add(detail);

                    alat.Stok -= 1;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Peminjaman berhasil diajukan! Tunggu verifikasi petugas.";

            return RedirectToPage();
        }
    }
}