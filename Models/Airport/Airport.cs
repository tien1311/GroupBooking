namespace EVCBooking.Models.Airport;

public partial class Airport
{
    public int Id { get; set; }

    public string AirportCode { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public string TimeZoneOffset { get; set; }

    public string IataCode { get; set; }

    public string CityCode { get; set; }

    public string CountryCode { get; set; }

    public string RegionCode { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public virtual List<AirportProfile> AirportProfiles { get; set; }
}
