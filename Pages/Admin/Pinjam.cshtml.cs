using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Pages.Admin
{
    public class PeminjamanModel : PageModel
    {
        private readonly AppDbContext _context;

        public PeminjamanModel(AppDbContext context)
        {
            _context = context;
        }

        public List<PeminjamanViewModel> DataPeminjaman { get; set; } = new();

        public List<User> ListUser { get; set; } = new();
        public List<Alat> ListAlat { get; set; } = new();

        [BindProperty]
        public Peminjaman Input { get; set; }

        [BindProperty]
        public List<int> SelectedAlat { get; set; } = new();

        [BindProperty]
        public int IdPeminjaman { get; set; }

        public void OnGet()
        {
            LoadData();
        }

        private void LoadData()
        {
            DataPeminjaman = _context.Peminjamans
                .Include(x => x.User)
                .Include(x => x.Details)
                .ThenInclude(x => x.Alat)
                .Select(p => new PeminjamanViewModel
                {
                    IdPeminjaman = p.IdPeminjaman,
                    NamaUser = p.User.Nama,
                    NamaAlat = string.Join(", ",
                        p.Details.Select(d => d.Alat.NamaAlat)),
                    TanggalPinjam = p.TanggalPinjam,
                    TanggalKembali = p.TanggalKembali,
                    Status = p.Status
                }).ToList();

            ListUser = _context.Users.ToList();

            ListAlat = _context.Alats
                .Where(x => x.Stok > 0)
                .ToList();
        }

        public IActionResult OnPostCreate()
        {
            Input.Status = "Pending";

            _context.Peminjamans.Add(Input);
            _context.SaveChanges();

            foreach (var idAlat in SelectedAlat)
            {
                _context.PeminjamanDetails.Add(new PeminjamanDetail
                {
                    IdPeminjaman = Input.IdPeminjaman,
                    IdAlat = idAlat,
                    Jumlah = 1
                });
            }

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostApprove()
        {
            var data = _context.Peminjamans
                .Include(x => x.Details)
                .ThenInclude(x => x.Alat)
                .FirstOrDefault(x => x.IdPeminjaman == IdPeminjaman);

            if (data == null) return RedirectToPage();

            foreach (var item in data.Details)
            {
                if (item.Alat.Stok > 0)
                {
                    item.Alat.Stok -= item.Jumlah;
                    item.Alat.UpdateStatus();
                }
            }

            data.Status = "Dipinjam";

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostReject()
        {
            var data = _context.Peminjamans
                .FirstOrDefault(x => x.IdPeminjaman == IdPeminjaman);

            if (data == null) return RedirectToPage();

            data.Status = "Ditolak";

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            var data = _context.Peminjamans
                .Include(x => x.Details)
                .FirstOrDefault(x => x.IdPeminjaman == IdPeminjaman);

            if (data == null) return RedirectToPage();

            _context.PeminjamanDetails.RemoveRange(data.Details);
            _context.Peminjamans.Remove(data);

            _context.SaveChanges();

            return RedirectToPage();
        }
    }

    public class PeminjamanViewModel
    {
        public int IdPeminjaman { get; set; }
        public string NamaUser { get; set; }
        public string NamaAlat { get; set; }
        public DateTime TanggalPinjam { get; set; }
        public DateTime TanggalKembali { get; set; }
        public string Status { get; set; }
    }
}