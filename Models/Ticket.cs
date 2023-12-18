namespace EVCBooking.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int Status { get; set; }

    public string Message { get; set; }

    public virtual List<Result> Results { get; set; }
}
