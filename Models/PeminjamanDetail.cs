namespace PeminjamanAlat.Models
{
    public class PeminjamanDetail
    {
        public int IdDetail { get; set; }
        public int IdPeminjaman { get; set; }
        public int IdAlat { get; set; }
        public int Jumlah { get; set; }
        public Peminjaman? Peminjaman { get; set; }
        public Alat? Alat { get; set; }
    }
}