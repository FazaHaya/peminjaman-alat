namespace PeminjamanAlat.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string Nama { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Peminjaman>? Peminjamans { get; set; }
        public ICollection<LogAktivitas>? Logs { get; set; }
        public ICollection<Denda>? Denda { get; set; }
    }
}