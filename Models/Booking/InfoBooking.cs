namespace EVCBooking.Models.Booking
{
    public class InfoBooking
    {
        public string ID { get; set; }
        public string AgentCode { get; set; }
        public string BookType { get; set; }
        public string Phone { get; set; }
        public string PhoneRemark { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public int NumberOfPassengers { get; set; }
        public double Fare { get; set; }
        public double Charge { get; set; }
        public double Price { get; set; }
        public List<Passenger> Passengers { get; set; }
        public List<Flight> Flights { get; set; }
        public double Total => NumberOfPassengers * (Price + Fare + Charge);
    }
}
