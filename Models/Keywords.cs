
namespace EVCBooking.Models
{
    public class Keywords
    {
        public string Airline { get; set; }
        public string BookType {get; set;}
        public string DepartureCode { get; set; }
        public string ArrivalCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public int Adult { get; set; }
        public int Children { get; set; }
        public int Baby { get; set; }
        public int NumberOfPassengers { get; set; }
        public int ViewOfMonth { get; set; }
    }
}
