namespace EVCBooking.Models;

public partial class Routes
{
    public int Id { get; set; }

    public string FlightNumber { get; set; }

    public DateTime FlightDate { get; set; }

    public string FlightHour { get; set; }

    public string WeekDay { get; set; }

    public string Resultid { get; set; }
}
