using System.ComponentModel.DataAnnotations.Schema;

namespace EVCBooking.Models.GroupBooking
{
    public class Result
    {
        public string id { get; set; }
        public string airline { get; set; }
        public string typeOfTrip { get; set; }
        public double price { get; set; }
        public double priceAgent { get; set; }
        public int numberOfGuests { get; set; }
        public string condition { get; set; }
        public int specification { get; set; }
        public string unit { get; set; }
        public List<Route> routes { get; set; }
        [NotMapped]
        public string general_condition { get; set; }
        [NotMapped]
        public string departure { get; set; }
        [NotMapped]
        public string arrival { get; set; }
        [NotMapped]
        public double total => numberOfGuests * price;
    }
}
