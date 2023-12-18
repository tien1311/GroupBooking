namespace EVCBooking.Models;

public partial class Result
{
    public string Id { get; set; }

    public string Airline { get; set; }

    public string Itinerary { get; set; }

    public double Price { get; set; }

    public double PriceAgent { get; set; }

    public string BookType { get; set; }

    public int NumberOfGuests { get; set; }

    public string Condition { get; set; }

    public int Specification { get; set; }

    public string Unit { get; set; }

    public double Fare { get; set; }

    public double Charge { get; set; }

    public double Total { get; set; }

    public int? Ticketid { get; set; }

    public virtual List<Routes> Routes { get; set; }

    public virtual Ticket Ticket { get; set; }
}
