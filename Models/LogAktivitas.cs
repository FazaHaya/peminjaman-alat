namespace PeminjamanAlat.Models
{
    public class LogAktivitas
    {
        public int IdLog { get; set; }
        public int IdUser { get; set; }
        public string Aktivitas { get; set; }
        public DateTime Waktu { get; set; }
        public User? User { get; set; }
    }
}