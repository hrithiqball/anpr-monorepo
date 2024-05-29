using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MlffWebApi.Database.DbContexts
{
    public partial class MlffDbContext : DbContext
    {
        public MlffDbContext()
        {
        }

        public MlffDbContext(DbContextOptions<MlffDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<detection_match> detection_matches { get; set; }
        public virtual DbSet<license_plate_recognition> license_plate_recognitions { get; set; }
        public virtual DbSet<rfid_detection> rfid_detections { get; set; }
        public virtual DbSet<site> sites { get; set; }
        public virtual DbSet<speed_detection> speed_detections { get; set; }
        public virtual DbSet<watchlist> watchlists { get; set; }
        public virtual DbSet<public_ip> public_ip_recognitions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<detection_match>(entity =>
            {
                entity.HasKey(e => e.uid)
                    .HasName("detection_match_pkey");

                entity.Property(e => e.uid).ValueGeneratedNever();
            });

            modelBuilder.Entity<license_plate_recognition>(entity =>
            {
                entity.HasKey(e => e.uid)
                    .HasName("license_plate_recognition_pkey");

                entity.HasIndex(e => e.date_detection, "license_plate_recognition_detection_date")
                    .HasSortOrder(new[] { SortOrder.Descending });

                entity.Property(e => e.uid).ValueGeneratedNever();
            });

            modelBuilder.Entity<rfid_detection>(entity =>
            {
                entity.HasKey(e => e.uid)
                    .HasName("rfid_detection_pkey");

                entity.HasIndex(e => e.date_detection, "rfid_detection_detection_date")
                    .HasSortOrder(new[] { SortOrder.Descending });

                entity.Property(e => e.uid).ValueGeneratedNever();
            });

            modelBuilder.Entity<speed_detection>(entity =>
            {
                entity.HasKey(e => e.uid)
                    .HasName("speed_detection_pkey");

                entity.HasIndex(e => e.date_detection, "speed_detection_detection_date")
                    .HasSortOrder(new[] { SortOrder.Descending });

                entity.Property(e => e.uid).ValueGeneratedNever();
            });

            modelBuilder.Entity<watchlist>(entity =>
            {
                entity.HasKey(e => e.uid)
                    .HasName("watchlist_pkey");

                entity.Property(e => e.uid).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
