namespace EVCBooking.Models.GroupBooking;

public partial class _Booking
{
    public int Id { get; set; }

    public string BookType { get; set; }

    public string Phone { get; set; }

    public string PhoneRemark { get; set; }

    public string Email { get; set; }

    public string Remark { get; set; }

    public int? NumberOfGuests { get; set; }

    public decimal? Total { get; set; }

    public string AgentCode { get; set; }

    public decimal? Fare { get; set; }

    public decimal? Charge { get; set; }

    public decimal? Price { get; set; }

    public int? IdFlight { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual List<_BookingFlight> BookingFlights { get; set; }

    public virtual List<_BookingPassenger> BookingPassengers { get; set; }
}
