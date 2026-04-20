using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.DTO;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogAktivitasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogAktivitasController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL LOG
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _context.LogAktivitas
                .Include(l => l.User)
                .OrderByDescending(l => l.Waktu)
                .Select(l => new LogAktivitasDTO
                {
                    IdLog = l.IdLog,
                    IdUser = l.IdUser,
                    NamaUser = l.User.Nama,
                    Aktivitas = l.Aktivitas,
                    Waktu = l.Waktu
                })
                .ToListAsync();

            return Ok(logs);
        }

        // GET LOG BY USER
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetLogUser(int id)
        {
            var logs = await _context.LogAktivitas
                .Include(l => l.User)
                .Where(l => l.IdUser == id)
                .OrderByDescending(l => l.Waktu)
                .Select(l => new LogAktivitasDTO
                {
                    IdLog = l.IdLog,
                    IdUser = l.IdUser,
                    NamaUser = l.User.Nama,
                    Aktivitas = l.Aktivitas,
                    Waktu = l.Waktu
                })
                .ToListAsync();

            return Ok(logs);
        }

        // CREATE LOG
        [HttpPost]
        public async Task<IActionResult> CreateLog(LogAktivitasCreateDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.IdUser == dto.IdUser);

            if (user == null)
                return BadRequest("User tidak ditemukan");

            var log = new LogAktivitas
            {
                IdUser = dto.IdUser,
                Aktivitas = dto.Aktivitas,
                Waktu = DateTime.Now
            };

            _context.LogAktivitas.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Log berhasil dibuat"
            });
        }
    }
}