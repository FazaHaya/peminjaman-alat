namespace PeminjamanAlat.Models
{
    public class Denda
    {
        public int id_denda { get; set; }
        public int id_pengembalian { get; set; }
        public int? id_user_konfirmasi { get; set; }
        public decimal? denda_terlambat { get; set; } = 0;
        public decimal? denda_kerusakan { get; set; } = 0;
        public decimal total_denda { get; set; } = 0;
        public string? status_pembayaran { get; set; } = "Belum Dibayar";
        public DateTime? tanggal_pembayaran { get; set; }

        public Pengembalian? Pengembalian { get; set; }
        public User? UserKonfirmasi { get; set; }
    }
}