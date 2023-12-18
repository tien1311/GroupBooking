namespace EVCBooking.Models.Booking
{
    public class Document
    {
        public string DocumentType { get; set; }
        public string BirthPlace { get; set; }
        public string IssuanceLocation { get; set; }
        public string IssuanceDate { get; set; }
        public string Number { get; set; }
        public string ExpiryDate { get; set; }
        public string IssuanceCountry { get; set; }
        public string ValidityCountry { get; set; }
        public string Nationality { get; set; }
        public bool Holder { get; set; }
    }
}
