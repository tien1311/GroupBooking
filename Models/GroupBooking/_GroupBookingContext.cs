using Microsoft.EntityFrameworkCore;

namespace EVCBooking.Models.GroupBooking;

public partial class _GroupBookingContext : DbContext
{
    public _GroupBookingContext()
    {
    }

    public _GroupBookingContext(DbContextOptions<_GroupBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<_Booking> Bookings { get; set; }

    public virtual DbSet<_BookingFlight> BookingFlights { get; set; }

    public virtual DbSet<_BookingPassenger> BookingPassengers { get; set; }

    public virtual DbSet<_Flight> Flights { get; set; }

    public virtual DbSet<_FlightDetail> FlightDetails { get; set; }

    public virtual DbSet<_PassengerDocument> PassengerDocuments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=42.117.7.18,1344;Database=GROUP_BOOKING;User=enviet;Password=Kythuat@123;TrustServerCertificate=Yes;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<_Booking>(entity =>
        {
            entity.ToTable("BOOKING");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AgentCode).HasMaxLength(100);
            entity.Property(e => e.BookType).HasMaxLength(50);
            entity.Property(e => e.Charge).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Fare).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.IdFlight).HasColumnName("ID_Flight");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PhoneRemark).HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.Total).HasColumnType("numeric(18, 0)");
        });

        modelBuilder.Entity<_BookingFlight>(entity =>
        {
            entity.ToTable("BOOKING_FLIGHT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AirlineSystem).HasMaxLength(20);
            entity.Property(e => e.ArrivalCode).HasMaxLength(20);
            entity.Property(e => e.DepartureCode).HasMaxLength(20);
            entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            entity.Property(e => e.FlightAirline).HasMaxLength(20);
            entity.Property(e => e.FlightCode).HasMaxLength(20);
            entity.Property(e => e.IdBooking).HasColumnName("ID_Booking");

            entity.HasOne(d => d.IdBookingNavigation).WithMany(p => p.BookingFlights)
                .HasForeignKey(d => d.IdBooking)
                .HasConstraintName("FK_BOOKING_FLIGHT_BOOKING1");
        });

        modelBuilder.Entity<_BookingPassenger>(entity =>
        {
            entity.ToTable("BOOKING_PASSENGER");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.IdBaggages).HasColumnName("ID_Baggages");
            entity.Property(e => e.IdBooking).HasColumnName("ID_Booking");
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.Title).HasMaxLength(20);
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.IdBookingNavigation).WithMany(p => p.BookingPassengers)
                .HasForeignKey(d => d.IdBooking)
                .HasConstraintName("FK_BOOKING_PASSENGER_BOOKING");
        });

        modelBuilder.Entity<_Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Flight");

            entity.ToTable("FLIGHT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Airline).HasMaxLength(50);
            entity.Property(e => e.Charge).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Fare).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Price).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.PriceAgent).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.TypeOfTrip).HasMaxLength(20);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedName).HasMaxLength(100);
        });

        modelBuilder.Entity<_FlightDetail>(entity =>
        {
            entity.ToTable("FLIGHT_DETAIL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ArrivalCode).HasMaxLength(20);
            entity.Property(e => e.DepartureCode).HasMaxLength(20);
            entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            entity.Property(e => e.DepartureHour)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.FlightCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.FlightId).HasColumnName("FlightID");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightDetails)
                .HasForeignKey(d => d.FlightId)
                .HasConstraintName("FK_FLIGHT_DETAIL_Flight");
        });

        modelBuilder.Entity<_PassengerDocument>(entity =>
        {
            entity.ToTable("PASSENGER_DOCUMENT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BirthPlace).HasMaxLength(50);
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IdPassenger).HasColumnName("ID_Passenger");
            entity.Property(e => e.IssuanceCountry).HasMaxLength(50);
            entity.Property(e => e.IssuanceDate).HasColumnType("datetime");
            entity.Property(e => e.IssuanceLocation).HasMaxLength(50);
            entity.Property(e => e.Nationality).HasMaxLength(50);
            entity.Property(e => e.Number)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ValidityCountry).HasMaxLength(50);

            entity.HasOne(d => d.IdPassengerNavigation).WithMany(p => p.PassengerDocuments)
                .HasForeignKey(d => d.IdPassenger)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PASSENGER_DOCUMENT_BOOKING_PASSENGER");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
