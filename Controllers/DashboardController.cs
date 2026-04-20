using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;

namespace PeminjamanAlat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var totalAlat = await _context.Alats.CountAsync();

            var alatTersedia = await _context.Alats
                .CountAsync(a => a.Status == "Tersedia");

            var alatDipinjam = await _context.Alats
                .CountAsync(a => a.Status == "Dipinjam");

            var totalPeminjaman = await _context.PeminjamanDetails.CountAsync();

            var totalPengembalian = await _context.Peminjamans
                .CountAsync(p => p.Status == "Dikembalikan");

            var aktivitasTerbaru = await _context.LogAktivitas
                .OrderByDescending(l => l.Waktu)
                .Take(5)
                .ToListAsync();

            var result = new
            {
                TotalAlat = totalAlat,
                AlatTersedia = alatTersedia,
                AlatDipinjam = alatDipinjam,
                TotalPeminjaman = totalPeminjaman,
                TotalPengembalian = totalPengembalian,
                AktivitasTerbaru = aktivitasTerbaru
            };

            return Ok(result);
        }
    }
}