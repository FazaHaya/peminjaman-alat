namespace PeminjamanAlat.NewFolder
{
    public class AlatDTO
    {
        public int IdAlat { get; set; }
        public int IdKategori { get; set; }
        public string NamaAlat { get; set; } = string.Empty;
        public int Stok { get; set; }
        public string Status { get; set; } = string.Empty;
        public string KodeAlat { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public string? Foto { get; set; }
        public string? NamaKategori { get; set; }
    }
    public class AlatCreateDTO
    {
        public int IdKategori { get; set; }
        public string NamaAlat { get; set; } = string.Empty;
        public int Stok { get; set; }
        public string Status { get; set; } = string.Empty;
        public string KodeAlat { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public string? Foto { get; set; }
    }
    public class AlatUpdateDTO : AlatCreateDTO;
}
