using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.DTO;
using PeminjamanAlat.Models;
using PeminjamanAlat.NewFolder;

namespace PeminjamanAlat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategoriController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KategoriController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL KATEGORI
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kategori = await _context.Kategoris
                .Select(k => new KategoriDTO
                {
                    IdKategori = k.IdKategori,
                    NamaKategori = k.NamaKategori,
                    TotalAlat = k.Alats.Count()
                })
                .ToListAsync();

            return Ok(kategori);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kategori = await _context.Kategoris
                .Include(k => k.Alats)
                .Where(k => k.IdKategori == id)
                .Select(k => new KategoriDTO
                {
                    IdKategori = k.IdKategori,
                    NamaKategori = k.NamaKategori,
                    TotalAlat = k.Alats.Count(),

                    Alats = k.Alats.Select(a => new AlatDTO
                    {
                        IdAlat = a.IdAlat,
                        NamaAlat = a.NamaAlat,
                        Stok = a.Stok,
                        Status = a.Status,
                        KodeAlat = a.KodeAlat,
                        Deskripsi = a.Deskripsi,
                        Foto = a.Foto
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (kategori == null)
                return NotFound("Kategori tidak ditemukan");

            return Ok(kategori);
        }

        // CREATE (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(KategoriCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.NamaKategori))
                return BadRequest("Nama kategori wajib diisi");

            bool exist = await _context.Kategoris
                .AnyAsync(k => k.NamaKategori == dto.NamaKategori);

            if (exist)
                return BadRequest("Kategori sudah ada");

            var kategori = new Kategori
            {
                NamaKategori = dto.NamaKategori
            };

            _context.Kategoris.Add(kategori);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Kategori berhasil ditambahkan",
                data = kategori
            });
        }

        // UPDATE (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, KategoriUpdateDTO dto)
        {
            var existing = await _context.Kategoris.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.NamaKategori = dto.NamaKategori;

            await _context.SaveChangesAsync();

            return Ok("Kategori berhasil diupdate");
        }

        // DELETE (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kategori = await _context.Kategoris.FindAsync(id);

            if (kategori == null)
                return NotFound();

            bool masihDipakai = await _context.Alats
                .AnyAsync(a => a.IdKategori == id);

            if (masihDipakai)
                return BadRequest("Kategori masih digunakan oleh alat");

            _context.Kategoris.Remove(kategori);
            await _context.SaveChangesAsync();

            return Ok("Kategori berhasil dihapus");
        }
    }
}