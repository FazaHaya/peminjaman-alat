namespace PeminjamanAlat.DTO
{
    public class PengembalianDTO
    {
        public int IdPengembalian { get; set; }
        public int IdPeminjaman { get; set; }
        public DateTime TanggalDikembalikan { get; set; }
        public decimal Denda { get; set; }
        public string KondisiAlat { get; set; } = string.Empty;
    }

    public class PengembalianCreateDTO
    {
        public int IdPeminjaman { get; set; }
        public string KondisiAlat { get; set; } = string.Empty;
    }
}