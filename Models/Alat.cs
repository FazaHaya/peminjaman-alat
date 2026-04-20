namespace PeminjamanAlat.Models
{
    public class Alat
    {
        public int IdAlat { get; set; }
        public int IdKategori { get; set; }
        public string NamaAlat { get; set; }
        public int Stok { get; set; }
        public string Status { get; set; }
        public string KodeAlat { get; set; }
        public string Deskripsi { get; set; }
        public string Foto { get; set; }

        public Kategori? Kategori { get; set; }
        public ICollection<PeminjamanDetail>? Details { get; set; }
        public void UpdateStatus()
        {
            Status = Stok > 0 ? "Tersedia" : "Dipinjam";
        }
    }
}