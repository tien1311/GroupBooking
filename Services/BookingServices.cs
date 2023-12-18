using EVCBooking.Models;
using EVCBooking.Models.Airport;
using EVCBooking.Models.Booking;
using EVCBooking.Models.GroupBooking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace EVCBooking.Services
{

    public class BookingServices
    {
        private readonly AirportContext airportContext;
        private readonly TicketContext ticketContext;
        private string token;
        
        public BookingServices(AirportContext _airportContext)
        {
            airportContext = _airportContext;   
            ticketContext = new TicketContext();
            token = GetToken().Result;
        }

        public async Task<string> GetToken()
        {
            using (HttpClient client = new HttpClient())
            {
                var data = new
                {
                    userName = "test",
                    passWord = "EnViet@123"
                };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri("https://api.evbay.vn/daily/Account/Authenticate");
                    request.Headers.Add("x-master-key", "1370542bcb7c6b71e34e975d4697f89bab164a520934ff47a5aceb780fdc92e6");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    var result = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject(result);
                    if (json != null)
                    {
                        return json["result"]["token"]; ;
                    }
                    else
                    {
                        return "Error token";
                    }
                }
            }
        }

        public async Task<Root> GetDataOfMonth(Object data)
        {
            using (var client = new HttpClient())
            {
                var jsonString = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("X-master-key", "1370542bcb7c6b71e34e975d4697f89bab164a520934ff47a5aceb780fdc92e6");
                var result = await client.PostAsync("https://api.evbay.vn/GroupBooking/Month/Search", content);
                string jsonResult = await result.Content.ReadAsStringAsync();
                Root root = JsonConvert.DeserializeObject<Root>(jsonResult);
                return root;
            }
        }


        public async Task<List<EVCBooking.Models.GroupBooking.Result>> GetDataById(string id)
        {
            using (var client = new HttpClient())
            {
                var data = new
                {
                    ID = id
                };
                var jsonString = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("X-master-key", "1370542bcb7c6b71e34e975d4697f89bab164a520934ff47a5aceb780fdc92e6");
                var result = await client.PostAsync("https://api.evbay.vn/GroupBooking/ID/Search", content);
                string jsonResult = await result.Content.ReadAsStringAsync();
                Root root = JsonConvert.DeserializeObject<Root>(jsonResult);
                return root.result;
            }
        }

        public async Task<string> GetCondition(string airline)
        {
            using (var client = new HttpClient())
            {
                var data = new
                {
                    PartnerCode = "",
                    AirlineIATACode = airline,
                    SeatClasses = "All",
                    CategoryName = airline + "DOAN"
                };
                var jsonString = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("X-master-key", "1370542bcb7c6b71e34e975d4697f89bab164a520934ff47a5aceb780fdc92e6");
                var result = await client.PostAsync("http://api.enviet-group.com/FareRules/Search", content);
                string jsonResult = await result.Content.ReadAsStringAsync();
                dynamic root = JsonConvert.DeserializeObject(jsonResult);
                if (root["result"].Count > 0)
                {
                    return root["result"][0]["domesticRules_vi"];
                }
                else
                {
                    return "";
                }
                
            }
        }

        public List<Object> SearchBookingApi(Keywords k)
        {
            List<Object> list = new List<Object>();

            string itinerary_oneway = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString();
            string itinerary_roundtrip = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString() + " - " + k.DepartureCode.ToString();
            var airlines = !string.IsNullOrEmpty(k.Airline) ? string.Join(",", k.Airline.Split(",")) : string.Join(",", new string[] { "VN", "VU", "QH", "VJ" });

            if (k.BookType == "ONEWAY") 
            {
                var data = new
                {
                    TypeOfTrip = k.BookType,
                    Month = k.DepartureDate.Month,
                    Year = k.DepartureDate.Year,
                    Airlines = airlines,
                    routes = new List<Object>() { new { RouteNo = 1, departureCode = k.DepartureCode, arrivalCode = k.ArrivalCode, departureDate = k.DepartureDate } }
                };
                   
                List<Models.GroupBooking.Result> q = GetDataOfMonth(data).Result.result;

                List<Models.GroupBooking.Result> q1 = new List<Models.GroupBooking.Result>();
                List<Models.GroupBooking.Result> q2 = new List<Models.GroupBooking.Result>();
                if (q.Exists(x => x.typeOfTrip == "ONEWAY"))
                {
                    foreach (var item in q)
                    {
                        if (item.routes.Exists(x => x.departureDate == k.DepartureDate))
                        {
                            item.general_condition = GetCondition(item.airline).Result;
                            item.departure = k.Departure;
                            q1.Add(item);
                        }
                        else
                        {
                            item.general_condition = GetCondition(item.airline).Result;
                            item.departure = k.Departure;
                            q2.Add(item);
                        }
                    }
                }    
                list.Add(q1);
                list.Add(q2);            
            }
            else
            {
                var data = new
                {
                    TypeOfTrip = k.BookType,
                    Month = k.DepartureDate.Month,
                    Year = k.DepartureDate.Year,
                    Airlines = airlines,
                    routes = new List<Object>() { 
                        new { RouteNo = 1, departureCode = k.DepartureCode, arrivalCode = k.ArrivalCode, departureDate = k.DepartureDate },
                        new { RouteNo = 2, departureCode = k.ArrivalCode, arrivalCode = k.DepartureCode, departureDate = k.ArrivalDate }
                    }
                };
                List<Models.GroupBooking.Result> q = GetDataOfMonth(data).Result.result;

                List<Models.GroupBooking.Result> q1 = new List<Models.GroupBooking.Result>();
                List<Models.GroupBooking.Result> q2 = new List<Models.GroupBooking.Result>();
                if (q.Exists(x => x.typeOfTrip == "ROUNDTRIP"))
                {
                    foreach (var item in q)
                    {
                        if (item.routes[0].departureDate == k.DepartureDate && item.routes[1].departureDate == k.ArrivalDate)
                        {
                            item.general_condition = GetCondition(item.airline).Result;
                            item.arrival = k.Arrival;
                            q1.Add(item);
                        }
                        else
                        {
                            item.general_condition = GetCondition(item.airline).Result;
                            item.arrival = k.Arrival;
                            q2.Add(item);
                        }
                    }
                }    
                list.Add(q1);
                list.Add(q2);
            }
            return list;
        }

        public Object ViewOfMonthApi(Keywords k)
        {
            List<Object> list = new List<Object>();

            string itinerary_oneway = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString();
            string itinerary_roundtrip = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString() + " - " + k.DepartureCode.ToString();
            var airlines = !string.IsNullOrEmpty(k.Airline) ? string.Join(",", k.Airline.Split(",")) : string.Join(",", new string[] { "VN", "VU", "QH", "VJ" });
            // ONEWAY
            if (k.BookType == "ONEWAY")
            {
                var data = new
                {
                    Month = k.DepartureDate.Month,
                    Year = k.DepartureDate.Year,
                    Airlines = airlines,
                    routes = new List<Object>() { new { RouteNo = 1, departureCode = k.DepartureCode, arrivalCode = k.ArrivalCode, departureDate = k.DepartureDate } }
                };

                List<Models.GroupBooking.Result> q = GetDataOfMonth(data).Result.result;
                Dictionary<int, Models.GroupBooking.Result> q1 = new Dictionary<int, Models.GroupBooking.Result>();
                if (q.Exists(x => x.typeOfTrip == "ONEWAY"))
                {
                    foreach (var item in q)
                    {
                        q1[item.routes[0].departureDate.Day] = item;
                    }
                }           
                list.Add(q1);
            }

            // ROUNDTRIP
            if (k.BookType == "ROUNDTRIP")
            {
                //Route 1
                var data = new
                {
                    Month = k.DepartureDate.Month,
                    Year = k.DepartureDate.Year,
                    Airlines = airlines,
                    routes = new List<Object>() {
                        new { RouteNo = 1, departureCode = k.DepartureCode, arrivalCode = k.ArrivalCode, departureDate = k.DepartureDate },
                        new { RouteNo = 2, departureCode = k.ArrivalCode, arrivalCode = k.DepartureCode, departureDate = k.ArrivalDate }
                    }
                };
                List<Models.GroupBooking.Result> q = GetDataOfMonth(data).Result.result;
                Dictionary<int, Models.GroupBooking.Result> q1 = new Dictionary<int, Models.GroupBooking.Result>();
                if (q.Exists(x => x.typeOfTrip == "ROUNDTRIP"))
                {
                    foreach (var item in q)
                    {
                        q1[item.routes[0].departureDate.Day] = item;
                    }
                }            
                list.Add(q1);

                // Route 2
                Dictionary<int, Models.GroupBooking.Result> q2 = new Dictionary<int, Models.GroupBooking.Result>();
                if (q.Exists(x => x.typeOfTrip == "ROUNDTRIP"))
                {
                    foreach (var item in q)
                    {
                        q2[item.routes[1].departureDate.Day] = item;
                    }
                }         
                list.Add(q2);
            }
            return list;
        }

        public List<Object> SearchBooking(Keywords k)
        {
            string itinerary_oneway = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString();
            string itinerary_roundtrip = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString() + " - " + k.DepartureCode.ToString();
            //var airlines = new List<string> { "VN", "VU", "QH", "VJ" };
            //var airlines = k.Airline.Split(",");
            //var airlines = !string.IsNullOrEmpty(k.Airline) ? k.Airline.Split(",") : new string[] { "VN", "VU", "QH", "VJ" };
            var airlines = !string.IsNullOrEmpty(k.Airline) ? string.Join(",", k.Airline.Split(",")) : string.Join(",", new string[] { "VN", "VU", "QH", "VJ" }); 

            List<Object> list = new List<Object>();

            if (k.BookType == "ONEWAY")
            {
                //Sử dụng AsNoTracking() để không theo dõi thay đổi dữ liệu khi chỉ cần đọc
                //Tìm chính xác theo ngày tháng trong keyword
                //var q1 = (from rs in ticketContext.Results
                //             join rt in ticketContext.Routes on rs.Id equals rt.Resultid
                //             where rt.FlightDate == k.DepartureDate && rs.Itinerary == itinerary_oneway && airlines.Contains(rs.Airline)
                //             group rs by new { rs.Id } into grouped
                //             where grouped.Count() == 1
                //             select new
                //             {
                //                 grouped.Key.Id
                //             }).AsNoTracking().ToList();

                //var q1 = from rs in ticketContext.Results
                //            join rt in ticketContext.Routes on rs.Id equals rt.Resultid
                //            where rs.Itinerary == itinerary_oneway 
                //            && airlines.Contains(rs.Airline)
                //            && rt.FlightDate.Year == k.DepartureDate.Year
                //            && rt.FlightDate.Month == k.DepartureDate.Month
                //            && rt.FlightDate.Day == k.DepartureDate.Day
                //            select new
                //            {
                //                rs.Id
                //            };

                var parameters = new[]{
                    //new SqlParameter("@type", "absolute"),                   
                    new SqlParameter("@itinerary", itinerary_oneway),
                    new SqlParameter("@airlines", airlines),
                    new SqlParameter("@year", k.DepartureDate.Year),
                    new SqlParameter("@month", k.DepartureDate.Month),
                    new SqlParameter("@day", k.DepartureDate.Day)
                };
                var list_01 = parameters.ToList();
                list_01.Insert(0, new SqlParameter("@type", "absolute"));
                // Set<Models.Store_Procedure.Result>() sử dụng khi kết quả trả về của store không đủ các cột trong entity class
                var q1 = ticketContext.Set<Models.Store_Procedure.Result>().FromSqlRaw($"SP_SEARCH_ONEWAY_FROM @type,@airlines,@itinerary,@year,@month,@day", list_01.ToArray())
                    .ToList();

                List<Models.Result> _Result1 = new List<Models.Result>();
                foreach (var item in q1)
                {
                    Models.Result result = new Models.Result();
                    result = ticketContext.Results.Where(x => x.Id == item.Id).First();
                    result.Routes = ticketContext.Routes.Where(x => x.Resultid == item.Id).ToList();
                    _Result1.Add(result);
                }
                list.Add(_Result1);

                //Tìm dữ liệu theo trong tháng ngoại trừ ngày đã chọn
                //var q2 = (from rs in ticketContext.Results
                //             join rt in ticketContext.Routes on rs.Id equals rt.Resultid
                //             where rt.FlightDate.Year == k.DepartureDate.Year && rt.FlightDate.Month == k.DepartureDate.Month && rt.FlightDate.Day != k.DepartureDate.Day && rs.Itinerary == itinerary_oneway && airlines.Contains(rs.Airline)
                //          group rs by new { rs.Id, rs.PriceAgent } into grouped
                //             where grouped.Count() == 1
                //             select new
                //             {
                //                 grouped.Key.Id,
                //                 grouped.Key.PriceAgent
                //             }).AsNoTracking().ToList();

                //var q2 = from rs in ticketContext.Results
                //         join rt in ticketContext.Routes on rs.Id equals rt.Resultid
                //         where rs.Itinerary == itinerary_oneway
                //         && airlines.Contains(rs.Airline)
                //         && rt.FlightDate.Year == k.DepartureDate.Year
                //         && rt.FlightDate.Month == k.DepartureDate.Month
                //         && rt.FlightDate.Day != k.DepartureDate.Day
                //         select new
                //         {
                //             rs.Id
                //         };

                var list_02 = parameters.ToList();
                list_02.Insert(0, new SqlParameter("@type", ""));
                // Set<Models.Store_Procedure.Result>() sử dụng khi kết quả trả về của store không đủ các cột trong entity class
                var q2 = ticketContext.Set<Models.Store_Procedure.Result>().FromSqlRaw($"SP_SEARCH_ONEWAY_FROM @type,@airlines,@itinerary,@year,@month,@day", list_02.ToArray())
                    .ToList();

                List<Models.Result> _Result2 = new List<Models.Result>();
                foreach (var item in q2)
                {
                    Models.Result result = new Models.Result();
                    result = ticketContext.Results.Where(x => x.Id == item.Id).First();
                    result.Routes = ticketContext.Routes.Where(x => x.Resultid == item.Id).ToList();
                    _Result2.Add(result);
                }
                list.Add(_Result2);

                return list;
            }
            else
            {

                //Tìm chính xác theo ngày tháng trong keyword
                //var q1 = (from rs in ticketContext.Results
                //          join rt in ticketContext.Routes on rs.Id equals rt.Resultid
                //          where rt.FlightDate == k.DepartureDate || rt.FlightDate == k.ArrivalDate && rs.Itinerary == itinerary_roundtrip && airlines.Contains(rs.Airline)
                //          group rs by new { rs.Id, rs.PriceAgent } into grouped
                //          where grouped.Count() == 2
                //          select new
                //          {
                //              grouped.Key.Id,
                //              grouped.Key.PriceAgent
                //          }).AsNoTracking().ToList();

                var parameters = new[]{
                    //new SqlParameter("@type", "absolute"),
                    new SqlParameter("@airlines", airlines),
                    new SqlParameter("@itinerary", itinerary_roundtrip),
                    new SqlParameter("@year_from", k.DepartureDate.Year),
                    new SqlParameter("@month_from", k.DepartureDate.Month),
                    new SqlParameter("@day_from", k.DepartureDate.Day),
                    new SqlParameter("@year_to", k.ArrivalDate.Value.Year),
                    new SqlParameter("@month_to", k.ArrivalDate.Value.Month),
                    new SqlParameter("@day_to", k.ArrivalDate.Value.Day)
                };
                var list_01 = parameters.ToList();
                list_01.Insert(0, new SqlParameter("@type", "absolute"));
                // Set<Models.Store_Procedure.Result>() sử dụng khi kết quả trả về của store không đủ các cột trong entity class
                var q1 = ticketContext.Set<Models.Store_Procedure.Result>().FromSqlRaw($"SP_SEARCH_ROUNDTRIP_FORM @type,@airlines,@itinerary,@year_from,@month_from,@day_from,@year_to,@month_to,@day_to", list_01.ToArray())
                    .ToList();

                List<Models.Result> _Result1 = new List<Models.Result>();
                foreach (var item in q1)
                {
                    Models.Result result = new Models.Result();
                    result = ticketContext.Results.Where(x => x.Id == item.Id).First();
                    result.Routes = ticketContext.Routes.Where(x => x.Resultid == item.Id).ToList();
                    _Result1.Add(result);
                }
                list.Add(_Result1);


                //Tìm dữ liệu theo tháng ngoại trừ ngày đã chọn             
                //var q2 = (from rs in ticketContext.Results
                //          join rt in ticketContext.Routes on rs.Id equals rt.Resultid
                //          where year_group.Contains(rt.FlightDate.Year) && month_group.Contains(rt.FlightDate.Month) && !day_group.Contains(rt.FlightDate.Day) && rs.Itinerary == itinerary_roundtrip && airlines.Contains(rs.Airline)
                //          group rs by new { rs.Id, rs.PriceAgent } into grouped
                //          where grouped.Count() == 2
                //          select new
                //          {
                //              grouped.Key.Id,
                //              grouped.Key.PriceAgent
                //          }).AsNoTracking().ToList();

                var list_02 = parameters.ToList();
                list_02.Insert(0, new SqlParameter("@type", ""));
                // Set<Models.Store_Procedure.Result>() sử dụng khi kết quả trả về của store không đủ các cột trong entity class
                var q2 = ticketContext.Set<Models.Store_Procedure.Result>().FromSqlRaw($"SP_SEARCH_ROUNDTRIP_FORM @type,@airlines,@itinerary,@year_from,@month_from,@day_from,@year_to,@month_to,@day_to", list_02.ToArray())
                    .ToList();

                List<Models.Result> _Result2 = new List<Models.Result>();
                foreach (var item in q2)
                {
                    Models.Result result = new Models.Result();
                    result = ticketContext.Results.Where(x => x.Id == item.Id).First();
                    result.Routes = ticketContext.Routes.Where(x => x.Resultid == item.Id).ToList();
                    _Result2.Add(result);
                }
                list.Add(_Result2);

                return list;
            }                               
        }

        public Object ViewOfMonth(Keywords k)
        {
            string itinerary_oneway = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString();
            string itinerary_roundtrip = k.DepartureCode.ToString() + " - " + k.ArrivalCode.ToString() + " - " + k.DepartureCode.ToString();
            var airlines = !k.Airline.IsNullOrEmpty() ? string.Join(",", k.Airline.ToString().Split(",")) : string.Join(",", new string[] { "VN", "VU", "QH", "VJ" });
            List<Object> list = new List<Object>();
            // ONEWAY
            if (k.BookType == "ONEWAY")
            {
                var parameters = new[]{
                    //new SqlParameter("@type", "absolute"),
                    new SqlParameter("@airlines", airlines),
                    new SqlParameter("@itinerary", itinerary_oneway),
                    new SqlParameter("@year", k.DepartureDate.Year),
                    new SqlParameter("@month", k.DepartureDate.Month),
                    new SqlParameter("@day", k.DepartureDate.Day)
                };
                var list_01 = parameters.ToList();
                list_01.Insert(0, new SqlParameter("@type", "all"));
                // Set<Models.Store_Procedure.Result>() sử dụng khi kết quả trả về của store không đủ các cột trong entity class
                var q1 = ticketContext.Set<Models.Store_Procedure.Result>().FromSqlRaw($"SP_SEARCH_ONEWAY_FROM @type,@airlines,@itinerary,@year,@month,@day", list_01.ToArray())
                    .ToList();

                Dictionary<int, Models.Result> _Result1 = new Dictionary<int, Models.Result>();
                foreach (var item in q1)
                {
                    Models.Result result = new Models.Result();
                    result = ticketContext.Results.Where(x => x.Id == item.Id).First();
                    result.Routes = ticketContext.Routes.Where(x => x.Resultid == item.Id).ToList();
                    _Result1[result.Routes[0].FlightDate.Day] = result;

                }
                list.Add(_Result1);

            }

            // ROUNDTRIP
            if (k.BookType == "ROUNDTRIP")
            {
                //Route 1
                var parameters = new[]{
                    //new SqlParameter("@type", "absolute"),
                    new SqlParameter("@airlines", airlines),
                    new SqlParameter("@itinerary", itinerary_roundtrip),
                    new SqlParameter("@year_from", k.DepartureDate.Year),
                    new SqlParameter("@month_from", k.DepartureDate.Month),
                    new SqlParameter("@day_from", k.DepartureDate.Day),
                    new SqlParameter("@year_to", k.ArrivalDate.Value.Year),
                    new SqlParameter("@month_to", k.ArrivalDate.Value.Month),
                    new SqlParameter("@day_to", k.ArrivalDate.Value.Day)
                };
                var list_01 = parameters.ToList();
                list_01.Insert(0, new SqlParameter("@type", "all"));
                // Set<Models.Store_Procedure.Result>() sử dụng khi kết quả trả về của store không đủ các cột trong entity class
                var q1 = ticketContext.Set<Models.Store_Procedure.Result>().FromSqlRaw($"SP_SEARCH_ROUNDTRIP_FORM @type,@airlines,@itinerary,@year_from,@month_from,@day_from,@year_to,@month_to,@day_to", list_01.ToArray())
                    .ToList();
                Dictionary<int, Models.Result> _Result1 = new Dictionary<int, Models.Result>();
                foreach (var item in q1)
                {
                    Models.Result result = new Models.Result();
                    result = ticketContext.Results.Where(x => x.Id == item.Id).First();
                    result.Routes = ticketContext.Routes.Where(x => x.Resultid == item.Id).ToList();
                    _Result1[result.Routes[0].FlightDate.Day] = result;
                }
                list.Add(_Result1);

                // Route 2
                var list_02 = parameters.ToList();
                list_02.Insert(0, new SqlParameter("@type", "all"));
                // Set<Models.Store_Procedure.Result>() sử dụng khi kết quả trả về của store không đủ các cột trong entity class
                var q2 = ticketContext.Set<Models.Store_Procedure.Result>().FromSqlRaw($"SP_SEARCH_ROUNDTRIP_FORM @type,@airlines,@itinerary,@year_from,@month_from,@day_from,@year_to,@month_to,@day_to", list_02.ToArray())
                    .ToList();
                Dictionary<int, Models.Result> _Result2 = new Dictionary<int, Models.Result>();
                foreach (var item in q2)
                {
                    Models.Result result = new Models.Result();
                    result = ticketContext.Results.Where(x => x.Id == item.Id).First();
                    result.Routes = ticketContext.Routes.Where(x => x.Resultid == item.Id).ToList();
                    _Result2[result.Routes[1].FlightDate.Day] = result;
                }
                list.Add(_Result2);
            }
            return list;
        }
        public async Task<string> InsertBooking(InfoBooking info, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                List<object> l_flights = new List<object>();
                List<object> l_passengers = new List<object>();
                for (var i = 0; i < info.Flights.Count; i++)
                {
                    var obj = new
                    {
                        RouteNo = info.Flights[i].RouteNo,
                        DepartureCode = info.Flights[i].DepartureCode,
                        ArrivalCode = info.Flights[i].ArrivalCode,
                        FlightCode = info.Flights[i].FlightCode,
                        DepartureDate = info.Flights[i].DepartureDate,
                        AirlineSystem = info.Flights[i].AirlineSystem,
                        FlightAirline = info.Flights[i].FlightAirline
                    };
                    l_flights.Add(obj);
                }

                List<object> l_documents = new List<object>();
                for (int i = 0; i < info.Passengers.Count; i++)
                {
                    for (int j = 0; j < info.Passengers[i].Documents.Count; j++)
                    {
                        var _obj = new
                        {
                            DocumentType = info.Passengers[i].Documents[j].DocumentType,
                            BirthPlace = info.Passengers[i].Documents[j].BirthPlace,
                            IssuanceLocation = info.Passengers[i].Documents[j].IssuanceLocation,
                            IssuanceDate = ExtensionHelper.Format_DateTime(info.Passengers[i].Documents[j].IssuanceDate),
                            Number = info.Passengers[i].Documents[j].Number,
                            ExpiryDate = ExtensionHelper.Format_DateTime(info.Passengers[i].Documents[j].ExpiryDate),
                            IssuanceCountry = info.Passengers[i].Documents[j].IssuanceCountry,
                            ValidityCountry = info.Passengers[i].Documents[j].ValidityCountry,
                            Nationality = info.Passengers[i].Documents[j].Nationality,
                            Holder = info.Passengers[i].Documents[j].Holder
                        };
                        l_documents.Add(_obj);
                    }

                    var obj = new
                    {
                        Type = info.Passengers[i].Type,
                        Title = info.Passengers[i].Title,
                        FirstName = info.Passengers[i].FirstName,
                        LastName = info.Passengers[i].LastName,
                        DateOfBirth = info.Passengers[i].DateOfBirth,
                        Documents = l_documents
                    };
                    l_passengers.Add(obj);
                }

                var data = new
                {
                    ID = info.ID,
                    AgentCode = info.AgentCode,
                    BookType = info.BookType,
                    Phone = info.Phone,
                    PhoneRemark = info.PhoneRemark,
                    Email = info.Email,
                    Remark = info.Remark,
                    NumberOfPassengers = info.NumberOfPassengers,
                    Fare = info.Fare,
                    Charge = info.Charge,
                    Price = info.Price,
                    Passengers = l_passengers,
                    Flights = l_flights,
                    Total = info.Total

                    //Profiles = new List<object>() { new JsonResult(new { LocaleId = airport.Profiles[0].LocaleId, AirportName = airport.Profiles[0].AirportName, Description = airport.Profiles[0].Description, CityName = airport.Profiles[0].CityName, CountryName = airport.Profiles[0].CountryName }).Value, new JsonResult(new { LocaleId = airport.Profiles[1].LocaleId, AirportName = airport.Profiles[1].AirportName, Description = airport.Profiles[1].Description, CityName = airport.Profiles[1].CityName, CountryName = airport.Profiles[1].CountryName }).Value }
                    //Profiles = new List<object>()
                    //{
                    //    new JsonResult(JsonConvert.SerializeObject(airport.Profiles[0])).Value,
                    //    new JsonResult(JsonConvert.SerializeObject(airport.Profiles[1])).Value
                    //}
                };
                var jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("X-master-key", "1370542bcb7c6b71e34e975d4697f89bab164a520934ff47a5aceb780fdc92e6");
                var response = await client.PostAsync("https://api.evbay.vn/GroupBooking/Booking/Create", content);
                if (response.IsSuccessStatusCode)
                {
                    return "Dữ liệu đã được chèn thành công.";
                }
                else
                {
                    return "Lỗi trong quá trình chèn dữ liệu: " + response.ReasonPhrase;
                }
            }
        }


        public async Task<string> InsertBookings(InfoBooking info, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                List<object> l_flights = new List<object>();
                List<object> l_passengers = new List<object>();
                for (var i = 0; i < info.Flights.Count; i++)
                {
                    var obj = new
                    {
                        RouteNo = info.Flights[i].RouteNo,
                        DepartureCode = info.Flights[i].DepartureCode,
                        ArrivalCode = info.Flights[i].ArrivalCode,
                        FlightCode = info.Flights[i].FlightCode,
                        DepartureDate = info.Flights[i].DepartureDate,
                        AirlineSystem = info.Flights[i].AirlineSystem,
                        FlightAirline = info.Flights[i].FlightAirline
                    };
                    l_flights.Add(obj);
                }

                List<object> l_documents = new List<object>();
                for (int i = 0; i < info.Passengers.Count; i++)
                {
                    //for (int j = 0; j < info.Passengers[i].Documents.Count; j++)
                    //{
                    //    var _obj = new
                    //    {
                    //        DocumentType = info.Passengers[i].Documents[j].DocumentType,
                    //        BirthPlace = info.Passengers[i].Documents[j].BirthPlace,
                    //        IssuanceLocation = info.Passengers[i].Documents[j].IssuanceLocation,
                    //        IssuanceDate = ExtensionHelper.Format_DateTime(info.Passengers[i].Documents[j].IssuanceDate),
                    //        Number = info.Passengers[i].Documents[j].Number,
                    //        ExpiryDate = ExtensionHelper.Format_DateTime(info.Passengers[i].Documents[j].ExpiryDate),
                    //        IssuanceCountry = info.Passengers[i].Documents[j].IssuanceCountry,
                    //        ValidityCountry = info.Passengers[i].Documents[j].ValidityCountry,
                    //        Nationality = info.Passengers[i].Documents[j].Nationality,
                    //        Holder = info.Passengers[i].Documents[j].Holder
                    //    };
                    //    l_documents.Add(_obj);
                    //}

                    var obj = new
                    {
                        Type = info.Passengers[i].Type,
                        Title = info.Passengers[i].Title,
                        FirstName = info.Passengers[i].FirstName,
                        LastName = info.Passengers[i].LastName,
                        DateOfBirth = "",
                        Documents = l_documents
                    };
                    l_passengers.Add(obj);
                }

                var data = new
                {
                    ID = info.ID,
                    AgentCode = info.AgentCode,
                    TypeOfTrip = info.BookType,
                    Phone = info.Phone,
                    PhoneRemark = info.PhoneRemark,
                    Email = info.Email,
                    Remark = info.Remark,
                    NumberOfPassengers = info.NumberOfPassengers,
                    Fare = info.Fare,
                    Charge = info.Charge,
                    Price = info.Price,
                    Passengers = l_passengers,
                    Flights = l_flights,
                    Total = info.Total
                };
                var jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("X-master-key", "1370542bcb7c6b71e34e975d4697f89bab164a520934ff47a5aceb780fdc92e6");
                var response = await client.PostAsync("https://api.evbay.vn/GroupBooking/Booking/Create", content);
                if (response.IsSuccessStatusCode)
                {
                    return "Đặt vé thành công.";
                }
                else
                {
                    return "Lỗi đặt vé: " + response.ReasonPhrase;
                }
            }
        }

        public Object List_Airports(string keyword)
        {
            var query = (from ap in airportContext.AirportProfiles
                            join a in airportContext.Airports on ap.AirportId equals a.Id
                            where ap.LocaleId == "VN"
                            group ap by new
                            {
                                a.IataCode,
                                ap.AirportName,
                                ap.CityName,
                                ap.CountryName
                            } into grouped
                            select new
                            {
                                grouped.Key.IataCode,
                                grouped.Key.AirportName,
                                grouped.Key.CityName,
                                grouped.Key.CountryName
                            }).Where(k => k.IataCode.Contains(keyword) || k.AirportName.Contains(keyword) || k.CityName.Contains(keyword) || k.CountryName.Contains(keyword)).OrderByDescending(m => m.IataCode).Take(10).ToList();
            return query;            
        }

        public Object List_Airports()
        {
            //var airport = new string[] { "SGN", "HAN" };
            var query = (from ap in airportContext.AirportProfiles
                            join a in airportContext.Airports on ap.AirportId equals a.Id
                            where ap.LocaleId == "VN" /*&& !airport.Contains(a.IataCode)*/
                            group ap by new
                            {
                                a.IataCode,
                                ap.AirportName,
                                ap.CityName,
                                ap.CountryName
                            } into grouped
                            select new
                            {
                                grouped.Key.IataCode,
                                grouped.Key.AirportName,
                                grouped.Key.CityName,
                                grouped.Key.CountryName
                            }).ToList();
            return query;
        }

        public Object List_Json_Airports()
        {
            //var airport = new string[] { "SGN", "HAN" };
            var query = (from ap in airportContext.AirportProfiles
                         join a in airportContext.Airports on ap.AirportId equals a.Id
                         where ap.LocaleId == "VN" /*&& !airport.Contains(a.IataCode)*/
                         group ap by new
                         {
                             a.IataCode,
                             //ap.AirportName,
                             ap.CityName,
                             //ap.CountryName
                         } into grouped
                         select new
                         {
                             grouped.Key.IataCode,
                             //grouped.Key.AirportName,
                             grouped.Key.CityName,
                             //grouped.Key.CountryName
                         }).ToList();
            return JsonConvert.SerializeObject(query);
        }

        public Object Popular_Airports()
        {
            var airport = new string[] { "HAN", "DAD", "DLI", "JFK", "LAX", "MNL", "SYD" };
            var query = (from ap in airportContext.AirportProfiles
                        join a in airportContext.Airports on ap.AirportId equals a.Id
                        where ap.LocaleId == "VN"
                        group ap by new { 
                            a.IataCode,
                            ap.AirportName,
                            ap.CityName,
                            ap.CountryName
                        } into grouped
                        select new
                        {
                            grouped.Key.IataCode,
                            grouped.Key.AirportName,
                            grouped.Key.CityName,
                            grouped.Key.CountryName
                        }).Where(x => airport.Contains(x.IataCode)).ToList();
            return query;                         
        }
      
    }
}

//- Thêm file DLL vào file .csproj của project để sử dụng:
//- Sử dụng Class Library để tạo file DLL


//<?xml version="1.0" encoding="utf-8"?>
//<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
//  <PropertyGroup>
//    <ActiveDebugProfile>https</ActiveDebugProfile>
//    <View_SelectedScaffolderID>RazorViewEmptyScaffolder</View_SelectedScaffolderID>
//    <View_SelectedScaffolderCategoryPath>root/Common/MVC/View</View_SelectedScaffolderCategoryPath>
//  </PropertyGroup>
  
//  Nội dung thêm nếu gặp lỗi thì xóa đoạn code dưới build và thêm lại
//  <ItemGroup>
//    <Reference Include="EVCLib">
//        <HintPath>lib\EVCLib.dll</HintPath>
//    </Reference>
//  </ItemGroup>

//</Project>