namespace PeminjamanAlat.Models
{
    public class Kategori
    {
        public int IdKategori { get; set; }
        public string NamaKategori { get; set; }

        public ICollection<Alat>? Alats { get; set; }
    }
}