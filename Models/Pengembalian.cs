namespace PeminjamanAlat.Models
{
    public class Pengembalian
    {
        public int IdPengembalian { get; set; }
        public int IdPeminjaman { get; set; }
        public DateTime TanggalDikembalikan { get; set; }
        public string KondisiKembali { get; set; }
        public Peminjaman? Peminjaman { get; set; }
        public ICollection<Denda>? Denda { get; set; }
    }
}