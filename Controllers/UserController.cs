using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Data;
using PeminjamanAlat.DTO;
using PeminjamanAlat.Models;

namespace PeminjamanAlat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL USER (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.IdUser,
                    u.Nama,
                    u.Username,
                    u.Role,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET USER BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Where(u => u.IdUser == id)
                .Select(u => new
                {
                    u.IdUser,
                    u.Nama,
                    u.Username,
                    u.Role,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User tidak ditemukan");

            return Ok(user);
        }

        // CREATE USER (ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDTO request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
                return BadRequest("Username sudah digunakan");

            var user = new User
            {
                Nama = request.Nama,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User berhasil dibuat",
                data = user
            });
        }

        // UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDTO request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            user.Nama = request.Nama;
            user.Role = request.Role;

            await _context.SaveChangesAsync();

            return Ok("User berhasil diupdate");
        }

        // DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            bool punyaPeminjaman =
                await _context.Peminjamans.AnyAsync(p => p.IdUser == id);

            if (punyaPeminjaman)
                return BadRequest("User masih memiliki riwayat peminjaman");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User berhasil dihapus");
        }
    }
}