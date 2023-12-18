using System;
using System.Collections.Generic;

namespace EVCBooking.Models.Airport;

public partial class AirportProfile
{
    public int Id { get; set; }

    public int AirportId { get; set; }

    public string LocaleId { get; set; }

    public string AirportName { get; set; }

    public string Description { get; set; }

    public string CityName { get; set; }

    public string CountryName { get; set; }

    public virtual Airport Airport { get; set; }
}
