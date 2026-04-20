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

        public List<ViewModel> DataPeminjaman { get; set; } = new();
        public List<User> ListUser { get; set; } = new();
        public List<Alat> ListAlat { get; set; } = new();

        [BindProperty]
        public Peminjaman Input { get; set; }

        [BindProperty]
        public int SelectedAlat { get; set; }

        [BindProperty]
        public int IdPeminjaman { get; set; }

        public class ViewModel
        {
            public int IdPeminjaman { get; set; }
            public string NamaUser { get; set; }
            public string NamaAlat { get; set; }
            public DateTime TanggalPinjam { get; set; }
            public DateTime TanggalKembali { get; set; }
            public string Status { get; set; }
        }

        public void OnGet()
        {
            LoadData();
        }

        void LoadData()
        {
            DataPeminjaman = _context.Peminjamans
            .Include(x => x.User)
            .Include(x => x.Details)
            .ThenInclude(x => x.Alat)
            .Select(x => new ViewModel
            {
                IdPeminjaman = x.IdPeminjaman,
                NamaUser = x.User.Nama,
                NamaAlat = string.Join(", ",
                    x.Details.Select(d => d.Alat.NamaAlat)),
                TanggalPinjam = x.TanggalPinjam,
                TanggalKembali = x.TanggalKembali,
                Status = x.Status
            }).ToList();

            ListUser = _context.Users
                .Where(x => x.Role == "Peminjam")
                .ToList();

            ListAlat = _context.Alats
                .Where(x => x.Stok > 0)
                .ToList();
        }

        public IActionResult OnPostCreate()
        {
            _context.Peminjamans.Add(Input);
            _context.SaveChanges();

            _context.PeminjamanDetails.Add(new PeminjamanDetail
            {
                IdPeminjaman = Input.IdPeminjaman,
                IdAlat = SelectedAlat,
                Jumlah = 1
            });

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var data = _context.Peminjamans
                .FirstOrDefault(x => x.IdPeminjaman == Input.IdPeminjaman);

            if (data == null) return RedirectToPage();

            data.TanggalPinjam = Input.TanggalPinjam;
            data.TanggalKembali = Input.TanggalKembali;
            data.Status = Input.Status;

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
}