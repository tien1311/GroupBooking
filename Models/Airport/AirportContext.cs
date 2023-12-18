using Microsoft.EntityFrameworkCore;

namespace EVCBooking.Models.Airport;

public partial class AirportContext : DbContext
{
    public AirportContext()
    {
    }

    public AirportContext(DbContextOptions<AirportContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<AirportProfile> AirportProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=42.117.7.18,1344;Database=INHOPDONG_V2;User=enviet;Password=Kythuat@123;TrustServerCertificate=Yes;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.ToTable("AIRPORT");

            entity.HasIndex(e => e.AirportCode, "AK_AirportCode").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AirportCode).HasMaxLength(10);
            entity.Property(e => e.CityCode).HasMaxLength(10);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IataCode).HasMaxLength(10);
            entity.Property(e => e.Latitude)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Longitude)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegionCode).HasMaxLength(10);
            entity.Property(e => e.TimeZoneOffset).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AirportProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AIRPORT___3214EC2775F81525");

            entity.ToTable("AIRPORT_PROFILE");

            entity.HasIndex(e => new { e.AirportId, e.LocaleId }, "UQ_dbo_AIRPORT_PROFILE_AirportID_LocaleID").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AirportId).HasColumnName("AirportID");
            entity.Property(e => e.AirportName).HasMaxLength(200);
            entity.Property(e => e.CityName).HasMaxLength(50);
            entity.Property(e => e.CountryName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.LocaleId)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false);

            entity.HasOne(d => d.Airport).WithMany(p => p.AirportProfiles)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AIRPORT_PROFILE_AIRPORT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
