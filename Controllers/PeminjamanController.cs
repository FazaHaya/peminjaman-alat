using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.DTO;
using PeminjamanAlat.Models;
using System.Security.Claims;

namespace PeminjamanAlat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeminjamanController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PeminjamanController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Peminjam")]
        [HttpPost("ajukan")]
        public async Task<IActionResult> AjukanPeminjaman(
            [FromBody] CreatePeminjamanRequest request)
        {
            if (request.Details == null || !request.Details.Any())
                return BadRequest("Detail alat wajib diisi");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var peminjaman = new Peminjaman
            {
                IdUser = userId,
                TanggalPinjam = request.TanggalPinjam,
                TanggalKembali = request.TanggalKembali,
                Status = "Pending",
                Catatan = request.Catatan
            };

            await _context.Peminjamans.AddAsync(peminjaman);
            await _context.SaveChangesAsync();

            foreach (var item in request.Details)
            {
                var alat = await _context.Alats.FindAsync(item.IdAlat);

                if (alat == null)
                    return BadRequest($"Alat ID {item.IdAlat} tidak ditemukan");

                if (alat.Stok < item.Jumlah)
                    return BadRequest($"Stok {alat.NamaAlat} tidak cukup");

                var detail = new PeminjamanDetail
                {
                    IdPeminjaman = peminjaman.IdPeminjaman,
                    IdAlat = item.IdAlat,
                    Jumlah = item.Jumlah
                };

                await _context.PeminjamanDetails.AddAsync(detail);
            }

            await _context.SaveChangesAsync();

            return Ok("Pengajuan peminjaman berhasil");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Peminjamans
                .Include(p => p.User)
                .Include(p => p.Details)
                    .ThenInclude(d => d.Alat)
                .Select(p => new PeminjamanDTO
                {
                    IdPeminjaman = p.IdPeminjaman,
                    IdUser = p.IdUser,
                    NamaUser = p.User.Nama,
                    Status = p.Status,
                    TanggalPinjam = p.TanggalPinjam,
                    TanggalKembali = p.TanggalKembali,
                    Jumlah = p.Details.Sum(d => d.Jumlah),
                    NamaAlat = string.Join(", ",
                        p.Details.Select(d => d.Alat.NamaAlat)),
                    IdAlat = p.Details.FirstOrDefault()!.IdAlat
                })
                .ToListAsync();

            return Ok(data);
        }

        // APPROVE PEMINJAMAN (PETUGAS)
        [Authorize(Roles = "Petugas")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var peminjaman = await _context.Peminjamans
                .Include(p => p.Details)
                .ThenInclude(d => d.Alat)
                .FirstOrDefaultAsync(p => p.IdPeminjaman == id);

            if (peminjaman == null)
                return NotFound();

            if (peminjaman.Status != "Pending")
                return BadRequest("Peminjaman sudah diproses");

            foreach (var detail in peminjaman.Details)
            {
                if (detail.Alat.Stok < detail.Jumlah)
                    return BadRequest($"Stok {detail.Alat.NamaAlat} habis");

                detail.Alat.Stok -= detail.Jumlah;
                detail.Alat.UpdateStatus();
            }

            peminjaman.Status = "Dipinjam";

            await _context.SaveChangesAsync();

            return Ok("Peminjaman disetujui");
        }

        // TOLAK PEMINJAMAN
        [Authorize(Roles = "Petugas")]
        [HttpPut("tolak/{id}")]
        public async Task<IActionResult> Tolak(int id)
        {
            var peminjaman = await _context.Peminjamans.FindAsync(id);

            if (peminjaman == null)
                return NotFound();

            if (peminjaman.Status != "Pending")
                return BadRequest("Sudah diproses");

            peminjaman.Status = "Ditolak";

            await _context.SaveChangesAsync();

            return Ok("Peminjaman ditolak");
        }
    }

    // REQUEST DTO
    public class CreatePeminjamanRequest
    {
        public DateTime TanggalPinjam { get; set; }
        public DateTime TanggalKembali { get; set; }
        public string? Catatan { get; set; }

        public List<DetailRequest> Details { get; set; }
            = new();
    }

    public class DetailRequest
    {
        public int IdAlat { get; set; }
        public int Jumlah { get; set; }
    }
}