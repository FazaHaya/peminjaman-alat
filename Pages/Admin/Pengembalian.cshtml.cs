using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Pages.Admin
{
    public class PengembalianModel : PageModel
    {
        private readonly AppDbContext _context;

        public PengembalianModel(AppDbContext context)
        {
            _context = context;
        }

        public class ViewModel
        {
            public int IdPengembalian { get; set; }
            public string NamaUser { get; set; }
            public string NamaAlat { get; set; }
            public DateTime Tanggal { get; set; }
            public string Kondisi { get; set; }
            public decimal TotalDenda { get; set; }
        }

        public List<ViewModel> DataPengembalian { get; set; } = new();
        public List<Peminjaman> ListPinjam { get; set; } = new();

        [BindProperty]
        public Pengembalian Input { get; set; }

        [BindProperty]
        public decimal NominalDenda { get; set; }

        [BindProperty]
        public int IdPengembalian { get; set; }

        public void OnGet()
        {
            LoadData();
        }

        void LoadData()
        {
            DataPengembalian = _context.Pengembalians
            .Include(x => x.Peminjaman)
            .ThenInclude(x => x.User)
            .Include(x => x.Peminjaman)
            .ThenInclude(x => x.Details)
            .ThenInclude(x => x.Alat)
            .Include(x => x.Denda)
            .Select(x => new ViewModel
            {
                IdPengembalian = x.IdPengembalian,
                NamaUser = x.Peminjaman.User.Nama,
                NamaAlat = string.Join(", ",
                    x.Peminjaman.Details
                    .Select(d => d.Alat.NamaAlat)),
                Tanggal = x.TanggalDikembalikan,
                Kondisi = x.KondisiKembali,
                TotalDenda = x.Denda
                    .Select(d => d.total_denda)
                    .FirstOrDefault()
            }).ToList();

            ListPinjam = _context.Peminjamans
                .Include(x => x.User)
                .Where(x => x.Status == "Dipinjam")
                .ToList();
        }

        public IActionResult OnPostCreate()
        {
            _context.Pengembalians.Add(Input);
            _context.SaveChanges();

            if (NominalDenda > 0)
            {
                _context.Dendas.Add(new Denda
                {
                    id_pengembalian = Input.IdPengembalian,
                    total_denda = NominalDenda
                });
            }

            var pinjam = _context.Peminjamans
                .Include(x => x.Details)
                .ThenInclude(x => x.Alat)
                .FirstOrDefault(x => x.IdPeminjaman == Input.IdPeminjaman);

            if (pinjam != null)
            {
                pinjam.Status = "Selesai";

                foreach (var d in pinjam.Details)
                {
                    d.Alat.Stok += d.Jumlah;
                    d.Alat.UpdateStatus();
                }
            }

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var data = _context.Pengembalians
                .FirstOrDefault(x => x.IdPengembalian == Input.IdPengembalian);

            if (data == null) return RedirectToPage();

            data.TanggalDikembalikan = Input.TanggalDikembalikan;
            data.KondisiKembali = Input.KondisiKembali;

            var denda = _context.Dendas
                .FirstOrDefault(x =>
                x.id_pengembalian == data.IdPengembalian);

            if (denda == null && NominalDenda > 0)
            {
                _context.Dendas.Add(new Denda
                {
                    id_pengembalian = data.IdPengembalian,
                    total_denda = NominalDenda
                });
            }
            else if (denda != null)
            {
                denda.total_denda = NominalDenda;
            }

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            var data = _context.Pengembalians
                .FirstOrDefault(x =>
                x.IdPengembalian == IdPengembalian);

            if (data == null) return RedirectToPage();

            var denda = _context.Dendas
                .Where(x => x.id_pengembalian == data.IdPengembalian);

            _context.Dendas.RemoveRange(denda);
            _context.Pengembalians.Remove(data);

            _context.SaveChanges();

            return RedirectToPage();
        }
    }
}