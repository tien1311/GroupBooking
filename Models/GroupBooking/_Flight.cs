namespace EVCBooking.Models.GroupBooking;

public partial class _Flight
{
    public int Id { get; set; }

    public string Airline { get; set; }
    public string TypeOfTrip { get; set; }

    public decimal? Price { get; set; }
    public decimal? PriceAgent { get; set; }

    public int? NumberOfGuests { get; set; }
    public string Condition { get; set; }
    public int? Specification { get; set; }
    public string Unit { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedName { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? Active { get; set; }


    public decimal? Fare { get; set; }

    public decimal? Charge { get; set; }

    public decimal? Discount { get; set; }


    public virtual List<_FlightDetail> FlightDetails { get; set; }
}
