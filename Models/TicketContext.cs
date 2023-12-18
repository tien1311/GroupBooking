using Microsoft.EntityFrameworkCore;

namespace EVCBooking.Models;

public partial class TicketContext : DbContext
{
    public TicketContext()
    {
    }

    public TicketContext(DbContextOptions<TicketContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Routes> Routes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    //Store Procedure Entity
    public virtual DbSet<Models.Store_Procedure.Result> StoreResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=42.117.7.18,1344;Database=BOOKING_FLIGHT;User=enviet;Password=Kythuat@123;TrustServerCertificate=Yes;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Result>(entity =>
        {
            entity.ToTable("RESULT");

            entity.HasIndex(e => e.Ticketid, "IX_RESULT_Ticketid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Airline).HasColumnName("airline");
            entity.Property(e => e.BookType).HasColumnName("bookType");
            entity.Property(e => e.Charge).HasColumnName("charge");
            entity.Property(e => e.Condition).HasColumnName("condition");
            entity.Property(e => e.Fare).HasColumnName("fare");
            entity.Property(e => e.Itinerary).HasColumnName("itinerary");
            entity.Property(e => e.NumberOfGuests).HasColumnName("numberOfGuests");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.PriceAgent).HasColumnName("priceAgent");
            entity.Property(e => e.Specification).HasColumnName("specification");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.Unit).HasColumnName("unit");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Results).HasForeignKey(d => d.Ticketid);
        });

        modelBuilder.Entity<Routes>(entity =>
        {
            entity.ToTable("ROUTES");

            entity.HasIndex(e => e.Resultid, "IX_ROUTES_Resultid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FlightDate).HasColumnName("flightDate");
            entity.Property(e => e.FlightHour).HasColumnName("flightHour");
            entity.Property(e => e.FlightNumber).HasColumnName("flightNumber");
            entity.Property(e => e.WeekDay).HasColumnName("weekDay");

        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("TICKET");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
