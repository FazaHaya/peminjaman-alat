using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;

namespace PeminjamanAlat.Pages.Admin
{
    public class LogModel : PageModel
    {
        private readonly AppDbContext _context;

        public LogModel(AppDbContext context)
        {
            _context = context;
        }

        public List<ViewModel> DataLog { get; set; } = new();

        public class ViewModel
        {
            public int IdLog { get; set; }
            public string NamaUser { get; set; }
            public string Aktivitas { get; set; }
            public DateTime Waktu { get; set; }
        }

        public void OnGet()
        {
            DataLog = _context.LogAktivitas
                .Include(x => x.User)
                .OrderByDescending(x => x.Waktu)
                .Select(x => new ViewModel
                {
                    IdLog = x.IdLog,
                    NamaUser = x.User != null
                        ? x.User.Nama
                        : "System",
                    Aktivitas = x.Aktivitas,
                    Waktu = x.Waktu
                })
                .ToList();
        }
    }
}