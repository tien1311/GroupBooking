namespace EVCBooking.Models.GroupBooking
{
    public class Root
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<Result> result { get; set; }
    }
}
