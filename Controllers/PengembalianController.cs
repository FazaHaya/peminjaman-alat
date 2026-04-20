using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PengembalianController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PengembalianController(AppDbContext context)
        {
            _context = context;
        }

        // USER AJUKAN PENGEMBALIAN
        [Authorize(Roles = "Peminjam")]

        [HttpPost("ajukan/{idPeminjaman}")]
        public IActionResult AjukanPengembalian(int idPeminjaman)
        {
            var peminjaman = _context.Peminjamans
                .FirstOrDefault(p => p.IdPeminjaman == idPeminjaman);

            if (peminjaman == null)
                return NotFound("Peminjaman tidak ditemukan");

            peminjaman.Status = "Menunggu Verifikasi";

            var pengembalian = new Pengembalian
            {
                IdPeminjaman = idPeminjaman,
                TanggalDikembalikan = DateTime.Now,
                KondisiKembali = "Belum Dicek"
            };

            var existing = _context.Pengembalians
                .FirstOrDefault(p => p.IdPeminjaman == idPeminjaman);

            if (existing != null)
                return BadRequest("Pengembalian sudah diajukan"); _context.SaveChanges();

            return Ok("Pengembalian diajukan");
        }

        // PETUGAS VERIFIKASI
        [Authorize(Roles = "Petugas")]
        [HttpPut("verifikasi/{idPengembalian}")]
        public IActionResult Verifikasi(int idPengembalian, [FromBody] VerifikasiRequest request)
        {
            var pengembalian = _context.Pengembalians
                .Include(p => p.Peminjaman)
                    .ThenInclude(p => p.Details)
                        .ThenInclude(d => d.Alat)
                .FirstOrDefault(p => p.IdPengembalian == idPengembalian);

            if (pengembalian == null)
                return NotFound();

            if (pengembalian.Peminjaman == null)
                return BadRequest("Data peminjaman tidak valid");

            // update kondisi + waktu real
            pengembalian.KondisiKembali = request.Kondisi;
            pengembalian.TanggalDikembalikan = DateTime.Now;

            foreach (var detail in pengembalian.Peminjaman.Details)
            {
                var alat = detail.Alat;

                alat.Stok += detail.Jumlah;
                alat.UpdateStatus();
            }

            decimal dendaTerlambat = 0;
            decimal dendaRusak = 0;

            if (pengembalian.TanggalDikembalikan >
                pengembalian.Peminjaman.TanggalKembali)
            {
                int telatHari =
                    (pengembalian.TanggalDikembalikan -
                    pengembalian.Peminjaman.TanggalKembali).Days;

                dendaTerlambat = telatHari * 5000;
            }

            if (request.Kondisi != "Baik")
            {
                dendaRusak = 50000;
            }

            if (dendaTerlambat > 0 || dendaRusak > 0)
            {
                _context.Dendas.Add(new Denda
                {
                    id_pengembalian = idPengembalian,
                    denda_terlambat = dendaTerlambat,
                    denda_kerusakan = dendaRusak,
                    total_denda = dendaTerlambat + dendaRusak,
                    status_pembayaran = "Belum Dibayar"
                });
            }

            pengembalian.Peminjaman.Status = "Selesai";

            _context.SaveChanges();

            return Ok("Pengembalian diverifikasi");
        }
    }

    public class VerifikasiRequest
    {
        public string Kondisi { get; set; }
    }
}