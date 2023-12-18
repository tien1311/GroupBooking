namespace EVCBooking.Models.GroupBooking;

public partial class _FlightDetail
{
    public int Id { get; set; }

    public int? FlightId { get; set; }

    public string FlightCode { get; set; }

    public DateTime? DepartureDate { get; set; }

    public string DepartureHour { get; set; }

    public bool? Status { get; set; }

    public int? KindFlight { get; set; }

    public string DepartureCode { get; set; }

    public string ArrivalCode { get; set; }

    public virtual _Flight Flight { get; set; }
}
