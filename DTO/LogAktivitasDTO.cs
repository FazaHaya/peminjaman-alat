namespace PeminjamanAlat.DTO
{
    public class LogAktivitasDTO
    {
        public int IdLog { get; set; }
        public int IdUser { get; set; }
        public string NamaUser { get; set; } = string.Empty;
        public string Aktivitas { get; set; } = string.Empty;
        public DateTime Waktu { get; set; }
    }

    public class LogAktivitasCreateDTO
    {
        public int IdUser { get; set; }
        public string Aktivitas { get; set; } = string.Empty;
    }
}