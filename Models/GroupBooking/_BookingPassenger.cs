namespace EVCBooking.Models.GroupBooking;

public partial class _BookingPassenger
{
    public int Id { get; set; }

    public string Type { get; set; }

    public string Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public int? IdBaggages { get; set; }

    public int? IdBooking { get; set; }

    public virtual _Booking IdBookingNavigation { get; set; }

    public virtual List<_PassengerDocument> PassengerDocuments { get; set; }
}
