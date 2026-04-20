namespace PeminjamanAlat.DTO
{
    public class PeminjamanDTO
    {
        public int IdPeminjaman { get; set; }
        public int IdUser { get; set; }
        public string NamaUser { get; set; } = string.Empty;
        public int IdAlat { get; set; }
        public string NamaAlat { get; set; } = string.Empty;
        public int Jumlah { get; set; }
        public DateTime TanggalPinjam { get; set; }
        public DateTime TanggalKembali { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PeminjamanCreateDTO
    {
        public int IdUser { get; set; }
        public int IdAlat { get; set; }
        public int Jumlah { get; set; }
        public DateTime TanggalKembali { get; set; }
    }
}