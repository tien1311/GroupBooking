using System.ComponentModel.DataAnnotations.Schema;

namespace EVCBooking.Models.GroupBooking
{
    public class Route
    {
        public string departureCode { get; set; }
        public string arrivalCode { get; set; }
        public string flightCode { get; set; }
        public DateTime departureDate { get; set; }
        public string departureHour { get; set; }
        public string weekDay { get; set; }
    }
}
