namespace EVCBooking.Models.GroupBooking;

public partial class _BookingFlight
{
    public int Id { get; set; }

    public int? RouteNo { get; set; }

    public string DepartureCode { get; set; }

    public string ArrivalCode { get; set; }

    public string FlightCode { get; set; }

    public DateTime? DepartureDate { get; set; }

    public string AirlineSystem { get; set; }

    public string FlightAirline { get; set; }

    public int? IdBooking { get; set; }

    public virtual _Booking IdBookingNavigation { get; set; }
}
