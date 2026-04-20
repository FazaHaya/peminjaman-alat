using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;
using PeminjamanAlat.NewFolder;

namespace PeminjamanAlat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlatController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL ALAT
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Alats
                .Select(a => new AlatDTO
                {
                    IdAlat = a.IdAlat,
                    IdKategori = a.IdKategori,
                    NamaAlat = a.NamaAlat,
                    Stok = a.Stok,
                    Status = a.Status,
                    KodeAlat = a.KodeAlat,
                    Deskripsi = a.Deskripsi,
                    Foto = a.Foto,
                    NamaKategori = a.Kategori.NamaKategori
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alat = await _context.Alats
                .Include(a => a.Kategori)
                .Where(a => a.IdAlat == id)
                .Select(a => new AlatDTO
                {
                    IdAlat = a.IdAlat,
                    IdKategori = a.IdKategori,
                    NamaAlat = a.NamaAlat,
                    Stok = a.Stok,
                    Status = a.Status,
                    KodeAlat = a.KodeAlat,
                    Deskripsi = a.Deskripsi,
                    Foto = a.Foto,
                    NamaKategori = a.Kategori.NamaKategori
                })
                .FirstOrDefaultAsync();

            if (alat == null)
                return NotFound("Alat tidak ditemukan");

            return Ok(alat);
        }

        // CREATE ALAT (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]AlatCreateDTO dto)
        {
            var alat = new Alat
            {
                IdKategori = dto.IdKategori,
                NamaAlat = dto.NamaAlat,
                Stok = dto.Stok,
                Status = dto.Status,
                KodeAlat = dto.KodeAlat,
                Deskripsi = dto.Deskripsi,
                Foto = dto.Foto
            };
            _context.Alats.Add(alat);
            await _context.SaveChangesAsync();
            return Ok(alat);
        }

        // UPDATE ALAT (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AlatUpdateDTO dto)
        {
            var alat = await _context.Alats.FindAsync(id);
            if (alat == null)
                return NotFound();
            alat.IdKategori = dto.IdKategori;
            alat.NamaAlat = dto.NamaAlat;
            alat.Stok = dto.Stok;
            alat.Status = dto.Status;
            alat.KodeAlat = dto.KodeAlat;
            alat.Deskripsi = dto.Deskripsi;
            alat.Foto = dto.Foto;

            await _context.SaveChangesAsync();

            return Ok("Alat berhasil diupdate");
        }

        // DELETE ALAT (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var alat = await _context.Alats.FindAsync(id);

            if (alat == null)
                return NotFound();

            _context.Alats.Remove(alat);
            await _context.SaveChangesAsync();

            return Ok("Alat berhasil dihapus");
        }
    }
}