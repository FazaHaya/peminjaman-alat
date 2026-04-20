using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PeminjamanAlat.Data;
using PeminjamanAlat.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Linq;

namespace PeminjamanAlat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == request.Username);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Username atau password salah");
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
        new Claim(ClaimTypes.Name, user.Nama),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var identity = new ClaimsIdentity(claims, "CookieAuth");

            await HttpContext.SignInAsync(
                "CookieAuth",
                new ClaimsPrincipal(identity)
            );

            return Ok(new
            {
                message = "Login berhasil",
                user = new
                {
                    user.IdUser,
                    user.Nama,
                    user.Username,
                    user.Role
                }
            });
        }

        [HttpPost("register-siswa")]
        public ActionResult RegisterSiswa([FromBody] RegisterRequest request)
        {
            // Validasi: Apakah semua field diisi
            if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.ConfirmPassword))
                return BadRequest(new { message = "Password tidak boleh kosong." });

            // Validasi: Apakah Password dan Confirm Password cocok?
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Password dan Konfirmasi Password tidak cocok!" });
            }

            // Validasi: Apakah username sudah ada
            if (_context.Users.Any(u => u.Username == request.Username))
                return BadRequest(new { message = "Username sudah digunakan." });

            // Proses Simpan
            var newUser = new User
            {
                Nama = request.Nama,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Peminjam",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { message = "Registrasi berhasil!" });
        }

        [HttpPut("reset-password/{id}")]
        public ActionResult ResetPassword(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound(new { message = "User tidak ditemukan." });

            // Reset ke default
            user.Password = BCrypt.Net.BCrypt.HashPassword("12345");
            _context.SaveChanges();

            return Ok(new { message = $"Password untuk {user.Nama} telah direset menjadi: 12345" });
        }

        [HttpPut("change-password/{id}")]
        public ActionResult ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound(new { message = "User tidak ditemukan." });

            // VALIDASI: Cek apakah password lama benar
            if (!BCrypt.Net.BCrypt.Verify(request.PasswordLama, user.Password))
            {
                return BadRequest(new { message = "Password lama salah!" });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.PasswordBaru);
            _context.SaveChanges();

            return Ok(new { message = "Password berhasil diperbarui." });
        }
        // Ambil Data Profil
        [HttpGet("profile/{id}")]
        public ActionResult GetProfile(int id)
        {
            var user = _context.Users
                .Where(u => u.IdUser == id)
                .Select(u => new { u.IdUser, u.Nama, u.Username, u.Role, u.CreatedAt })
                .FirstOrDefault();

            if (user == null) return NotFound();
            return Ok(user);
        }

        // Update Data Profil (Nama & Username)
        [HttpPut("profile/update/{id}")]
        public ActionResult UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            // Cek username unik
            if (_context.Users.Any(u => u.Username == request.Username && u.IdUser != id))
                return BadRequest(new { message = "Username sudah dipakai." });

            user.Nama = request.Nama;
            user.Username = request.Username;

            _context.SaveChanges();
            return Ok(new { message = "Profil berhasil diupdate!" });
        }
    }
    public class UpdateProfileRequest
    {
        public string Nama { get; set; }
        public string Username { get; set; }
    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string PasswordLama { get; set; }
        public string PasswordBaru { get; set; }
    }
    public class RegisterRequest
    {
        public string Nama { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}