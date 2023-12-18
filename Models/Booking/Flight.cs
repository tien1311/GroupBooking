namespace EVCBooking.Models.Booking
{
    public class Flight
    {
        public int RouteNo { get; set; }
        public string DepartureCode { get; set; }
        public string ArrivalCode { get; set; }
        public string FlightCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public string AirlineSystem { get; set; }
        public string FlightAirline { get; set; }
    }
}
