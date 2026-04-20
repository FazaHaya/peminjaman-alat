namespace PeminjamanAlat.Models
{
    public class Peminjaman
    {
        public int IdPeminjaman { get; set; }
        public int IdUser { get; set; }
        public DateTime TanggalPinjam { get; set; }
        public DateTime TanggalKembali { get; set; }
        public string? Status { get; set; }
        public string? Catatan { get; set; }
        public User? User { get; set; }
        public ICollection<PeminjamanDetail>? Details { get; set; }
        public Pengembalian? Pengembalian { get; set; }
    }
}