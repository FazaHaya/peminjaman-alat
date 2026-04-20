using Microsoft.EntityFrameworkCore;
using PeminjamanAlat.Models;
using System;

namespace PeminjamanAlat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Kategori> Kategoris { get; set; }
        public DbSet<Alat> Alats { get; set; }
        public DbSet<Peminjaman> Peminjamans { get; set; }
        public DbSet<PeminjamanDetail> PeminjamanDetails { get; set; }
        public DbSet<Pengembalian> Pengembalians { get; set; }
        public DbSet<LogAktivitas> LogAktivitas { get; set; }
        public DbSet<Denda> Dendas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-CSK0UVJ;Database=Inventory;Trusted_Connection=True;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alat>(entity =>
            {
                entity.HasKey(e => e.IdAlat).HasName("PK__Alat__8A132BBAFF03AF94");

                entity.ToTable("Alat");

                entity.Property(e => e.IdAlat).HasColumnName("id_alat");
                entity.Property(e => e.Deskripsi).HasColumnName("deskripsi");
                entity.Property(e => e.Foto)
                    .HasMaxLength(255)
                    .HasColumnName("foto");
                entity.Property(e => e.IdKategori).HasColumnName("id_kategori");
                entity.Property(e => e.KodeAlat)
                    .HasMaxLength(255)
                    .HasColumnName("kode_alat");
                entity.Property(e => e.NamaAlat)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nama_alat");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Stok).HasColumnName("stok");

                entity.HasOne(d => d.Kategori).WithMany(p => p.Alats)
                    .HasForeignKey(d => d.IdKategori)
                    .HasConstraintName("FK_Alat_Kategori");
            });

            modelBuilder.Entity<Denda>(entity =>
            {
                entity.HasKey(e => e.id_denda).HasName("PK__Denda__12634E37DBAD8D84");

                entity.ToTable("Denda");

                entity.Property(e => e.id_denda).HasColumnName("id_denda");
                entity.Property(e => e.denda_kerusakan)
                    .HasDefaultValue(0m)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("denda_kerusakan");
                entity.Property(e => e.denda_terlambat)
                    .HasDefaultValue(0m)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("denda_terlambat");
                entity.Property(e => e.id_pengembalian).HasColumnName("id_pengembalian");
                entity.Property(e => e.id_user_konfirmasi).HasColumnName("id_user_konfirmasi");
                entity.Property(e => e.status_pembayaran)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Belum Bayar")
                    .HasColumnName("status_pembayaran");
                entity.Property(e => e.tanggal_pembayaran)
                    .HasColumnType("datetime")
                    .HasColumnName("tanggal_pembayaran");
                entity.Property(e => e.total_denda)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_denda");

                entity.HasOne(d => d.Pengembalian)
                    .WithMany(p => p.Denda)
                    .HasForeignKey(d => d.id_pengembalian)
                    .HasConstraintName("FK_Denda_Pengembalian");

                entity.HasOne(d => d.UserKonfirmasi)
                    .WithMany(p => p.Denda)
                    .HasForeignKey(d => d.id_user_konfirmasi)
                    .HasConstraintName("FK_Denda_Petugas");
            });

            modelBuilder.Entity<PeminjamanDetail>(entity =>
            {
                entity.HasKey(e => e.IdDetail).HasName("PK__Detail_P__EA8338085E969559");

                entity.ToTable("Detail_Peminjaman");

                entity.Property(e => e.IdDetail).HasColumnName("id_detail");
                entity.Property(e => e.IdAlat).HasColumnName("id_alat");
                entity.Property(e => e.IdPeminjaman).HasColumnName("id_peminjaman");
                entity.Property(e => e.Jumlah).HasColumnName("jumlah");

                entity.HasOne(d => d.Peminjaman)
                    .WithMany(p => p.Details)
                    .HasForeignKey(d => d.IdPeminjaman)
                    .HasConstraintName("FK_Detail_Peminjaman");

                entity.HasOne(d => d.Alat)
                    .WithMany(a => a.Details)
                    .HasForeignKey(d => d.IdAlat)
                    .HasConstraintName("FK_Detail_Alat");
            });

            modelBuilder.Entity<Kategori>(entity =>
            {
                entity.HasKey(e => e.IdKategori).HasName("PK__Kategori__749DC5C833E1F3A1");

                entity.ToTable("Kategori");

                entity.Property(e => e.IdKategori).HasColumnName("id_kategori");
                entity.Property(e => e.NamaKategori)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nama_kategori");
            });

            modelBuilder.Entity<LogAktivitas>(entity =>
            {
                entity.HasKey(e => e.IdLog).HasName("PK__Log_Akti__6CC851FE78447787");

                entity.ToTable("Log_Aktivitas");

                entity.Property(e => e.IdLog).HasColumnName("id_log");
                entity.Property(e => e.Aktivitas)
                    .HasColumnType("text")
                    .HasColumnName("aktivitas");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.Waktu)
                    .HasColumnType("datetime")
                    .HasColumnName("waktu");

                entity.HasOne(d => d.User).WithMany(p => p.Logs) 
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Log_User");
            });

            modelBuilder.Entity<Peminjaman>(entity =>
            {
                entity.HasKey(e => e.IdPeminjaman).HasName("PK__Peminjam__546CC69EB3874A64");

                entity.ToTable("Peminjaman");

                entity.Property(e => e.IdPeminjaman).HasColumnName("id_peminjaman");
                entity.Property(e => e.Catatan).HasColumnName("catatan");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");
                entity.Property(e => e.TanggalKembali)
                    .HasColumnType("datetime")
                    .HasColumnName("tanggal_kembali");
                entity.Property(e => e.TanggalPinjam)
                    .HasColumnType("datetime")
                    .HasColumnName("tanggal_pinjam");

                entity.HasOne(d => d.User).WithMany(p => p.Peminjamans)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Peminjaman_User");
            });

            modelBuilder.Entity<Pengembalian>(entity =>
            {
                entity.HasKey(e => e.IdPengembalian).HasName("PK__Pengemba__9E1B8DAE72EF1F17");

                entity.ToTable("Pengembalian");

                entity.HasIndex(e => e.IdPeminjaman, "UQ__Pengemba__546CC69F8CC8BCE6").IsUnique();

                entity.Property(e => e.IdPengembalian).HasColumnName("id_pengembalian");
                entity.Property(e => e.IdPeminjaman).HasColumnName("id_peminjaman");
                entity.Property(e => e.KondisiKembali)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("kondisi_kembali");
                entity.Property(e => e.TanggalDikembalikan)
                    .HasColumnType("datetime")
                    .HasColumnName("tanggal_dikembalikan");

                entity.HasOne(d => d.Peminjaman).WithOne(p => p.Pengembalian)
                    .HasForeignKey<Pengembalian>(d => d.IdPeminjaman)
                    .HasConstraintName("FK_Pengembalian_Peminjaman");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser).HasName("PK__User__D2D14637F32BA05E");

                entity.ToTable("User");

                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.Nama)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nama");
                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");
                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("role");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            //OnModelCreatingPartial(modelBuilder);
        }
        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}