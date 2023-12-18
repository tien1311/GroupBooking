namespace EVCBooking.Models.GroupBooking;

public partial class _PassengerDocument
{
    public int Id { get; set; }

    public int IdPassenger { get; set; }

    public string DocumentType { get; set; }

    public string BirthPlace { get; set; }

    public string IssuanceLocation { get; set; }

    public DateTime? IssuanceDate { get; set; }

    public string Number { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string IssuanceCountry { get; set; }

    public string ValidityCountry { get; set; }

    public string Nationality { get; set; }

    public bool? Holder { get; set; }

    public virtual _BookingPassenger IdPassengerNavigation { get; set; }
}
