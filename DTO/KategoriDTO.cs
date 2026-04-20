using PeminjamanAlat.Models;
using PeminjamanAlat.NewFolder;

namespace PeminjamanAlat.DTO
{
    public class KategoriDTO
    {
        public int IdKategori { get; set; }
        public string NamaKategori { get; set; } = string.Empty;
        public int TotalAlat { get; set; }
        public ICollection<AlatDTO> Alats { get; set; }
    }
    public class KategoriCreateDTO
    {
        public string NamaKategori { get; set; } = string.Empty;
    }
    public class KategoriUpdateDTO : KategoriCreateDTO;
}
