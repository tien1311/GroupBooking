using EVCBooking.Models;
using EVCBooking.Models.Airport;
using EVCBooking.Models.Booking;
using EVCBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;


namespace EVCBooking.Controllers
{

    public class HomeController : Controller
    {
        BookingServices bks;
        public string token;
        private readonly TicketContext ticketContext;
        public HomeController(AirportContext airportContext) {
            ticketContext = new TicketContext();
            bks = new BookingServices(airportContext);
            token = EVCLib.EVCLib.GetToken().Result;
        }

        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, VaryByHeader = "Accept-Encoding")] //Cache này sẽ phụ thuộc vào giá trị của header "Accept-Encoding". 
        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, VaryByHeader = "User-Agent")] //Caching theo dõi thay đổi
        public IActionResult Index()
        {           
            return View();
        }


        //public IActionResult Search()
        //{
        //    Keywords keyword = new Keywords();
        //    keyword.Airline = HttpContext.Request.Query["Airline"];
        //    keyword.BookType = HttpContext.Request.Query["BookType"];
        //    keyword.DepartureCode = HttpContext.Request.Query["DepartureCode"];
        //    keyword.ArrivalCode = HttpContext.Request.Query["ArrivalCode"];
        //    keyword.DepartureDate = ExtensionHelper.Format_DateTime(HttpContext.Request.Query["DepartureDate"]);
        //    if (keyword.BookType == "ONEWAY")
        //    {
        //        keyword.ArrivalDate = ExtensionHelper.Format_DateTime(HttpContext.Request.Query["DepartureDate"]);
        //    }
        //    else
        //    {
        //        keyword.ArrivalDate = ExtensionHelper.Format_DateTime(HttpContext.Request.Query["ArrivalDate"]);
        //    }
        //    keyword.Adult = int.Parse(HttpContext.Request.Query["adult"]);
        //    keyword.Children = int.Parse(HttpContext.Request.Query["child"]);
        //    keyword.Baby = int.Parse(HttpContext.Request.Query["baby"]);
        //    keyword.NumberOfPassengers = keyword.Adult + keyword.Children + keyword.Baby;
        //    keyword.ViewOfMonth = HttpContext.Request.Query["ViewOfMonth"] != null ? int.Parse(HttpContext.Request.Query["ViewOfMonth"]) : 0;
        //    ViewBag.keyword = keyword;
        //    ViewBag.data = bks.SearchBooking(keyword);
        //    ViewBag.list_airport = bks.List_Airports();
        //    return View();
        //}



        //Khi dùng Ajax để gửi {jsonString : $(this).serialize()} thì phải dùng đúng tên jsonString cho tham số trong Action mới lấy được data        
        public IActionResult LoadPartialDataSearch(string jsonString)
        {
            // Deserializes chuỗi JSON thành dynamic object khi không có class model nào phù hợp hoặc chưa tạo dc class phù hợp
            dynamic obj = JsonConvert.DeserializeObject(jsonString);
            Keywords keyword = new Keywords();
            keyword.Airline = obj.Airline;
            keyword.BookType = obj.BookType;
            keyword.DepartureCode = obj.DepartureCode;
            keyword.ArrivalCode = obj.ArrivalCode;
            keyword.DepartureDate = ExtensionHelper.Format_DateTime(obj.DepartureDate.ToString());
            if (keyword.BookType == "ONEWAY")
            {
                keyword.ArrivalDate = ExtensionHelper.Format_DateTime(obj.DepartureDate.ToString());
            }
            else
            {
                keyword.ArrivalDate = ExtensionHelper.Format_DateTime(obj.ArrivalDate.ToString());
            }
            keyword.Adult = int.Parse(obj.adult.ToString());
            keyword.Children = int.Parse(obj.child.ToString());
            keyword.Baby = int.Parse(obj.baby.ToString());
            keyword.NumberOfPassengers = keyword.Adult + keyword.Children + keyword.Baby;
            keyword.ViewOfMonth = obj.ViewOfMonth != null ? int.Parse(obj.ViewOfMonth.ToString()): 0;
           
            //Nếu truyền data cho PartialView qua tuple
            //var tuple = new Tuple<Keywords, Object>(keyword, ViewBag.data);
            //return PartialView("_Partial_Data_Search", tuple);
            //Truyền trực tiếp qua ViewBag
            if (keyword.ViewOfMonth == 1)
            {
                //Route 1
                int year_r1 = keyword.DepartureDate.Year;
                int month_r1 = keyword.DepartureDate.Month;
                DateTime firstDayOfMonth_r1 = new DateTime(year_r1, month_r1, 1);         
                ViewBag.monthOfYear_r1 = $"{month_r1}/{year_r1}";
                ViewBag.firstDayOfMonth_r1 = (int)firstDayOfMonth_r1.DayOfWeek;
                ViewBag.daysInMonth_r1 = DateTime.DaysInMonth(year_r1, month_r1);

                if (keyword.BookType == "ROUNDTRIP")
                {
                    //Route 2
                    int year_r2 = keyword.ArrivalDate.Value.Year;
                    int month_r2 = keyword.ArrivalDate.Value.Month;
                    DateTime firstDayOfMonth_r2 = new DateTime(year_r2, month_r2, 1);
                    ViewBag.monthOfYear_r2 = $"{month_r2}/{year_r2}";
                    ViewBag.firstDayOfMonth_r2 = (int)firstDayOfMonth_r2.DayOfWeek;
                    ViewBag.daysInMonth_r2 = DateTime.DaysInMonth(year_r2, month_r2);
                }          
                            
                ViewBag.keyword = keyword;
                ViewBag.data = bks.ViewOfMonth(keyword);              
                return PartialView("_Partial_Data_Month");             
            }
            else
            {

                ViewBag.keyword = keyword;
                ViewBag.data = bks.SearchBookingApi(keyword);
                //ViewBag.data = bks.SearchBooking(keyword);       
                return PartialView("_Partial_Data_Search");
            }               
        }


        public IActionResult LoadPartialDataSearch_New(string jsonString)
        {
            // Deserializes chuỗi JSON thành dynamic object khi không có class model nào phù hợp hoặc chưa tạo dc class phù hợp
            dynamic obj = JsonConvert.DeserializeObject(jsonString);
            Keywords keyword = new Keywords();
            keyword.Airline = obj.Airline;
            keyword.BookType = obj.BookType;
            keyword.DepartureCode = obj.DepartureCode;
            keyword.ArrivalCode = obj.ArrivalCode;
            keyword.Departure = obj.Departure;
            keyword.Arrival = obj.Arrival;
            keyword.DepartureDate = ExtensionHelper.Format_DateTime(obj.DepartureDate.ToString());
            if (keyword.BookType == "ONEWAY")
            {
                keyword.ArrivalDate = ExtensionHelper.Format_DateTime(obj.DepartureDate.ToString());
            }
            else
            {
                keyword.ArrivalDate = ExtensionHelper.Format_DateTime(obj.ArrivalDate.ToString());
            }
            keyword.Adult = int.Parse(obj.adult.ToString());
            keyword.Children = int.Parse(obj.child.ToString());
            keyword.Baby = int.Parse(obj.baby.ToString());
            keyword.NumberOfPassengers = keyword.Adult + keyword.Children + keyword.Baby;
            keyword.ViewOfMonth = obj.ViewOfMonth != null ? int.Parse(obj.ViewOfMonth.ToString()) : 0;

            //Nếu truyền data cho PartialView qua tuple
            //var tuple = new Tuple<Keywords, Object>(keyword, ViewBag.data);
            //return PartialView("_Partial_Data_Search", tuple);
            //Truyền trực tiếp qua ViewBag
            if (keyword.ViewOfMonth == 1)
            {
                //Route 1
                int year_r1 = keyword.DepartureDate.Year;
                int month_r1 = keyword.DepartureDate.Month;
                DateTime firstDayOfMonth_r1 = new DateTime(year_r1, month_r1, 1);
                ViewBag.monthOfYear_r1 = $"{month_r1}/{year_r1}";
                ViewBag.firstDayOfMonth_r1 = (int)firstDayOfMonth_r1.DayOfWeek;
                ViewBag.daysInMonth_r1 = DateTime.DaysInMonth(year_r1, month_r1);

                if (keyword.BookType == "ROUNDTRIP")
                {
                    //Route 2
                    int year_r2 = keyword.ArrivalDate.Value.Year;
                    int month_r2 = keyword.ArrivalDate.Value.Month;
                    DateTime firstDayOfMonth_r2 = new DateTime(year_r2, month_r2, 1);
                    ViewBag.monthOfYear_r2 = $"{month_r2}/{year_r2}";
                    ViewBag.firstDayOfMonth_r2 = (int)firstDayOfMonth_r2.DayOfWeek;
                    ViewBag.daysInMonth_r2 = DateTime.DaysInMonth(year_r2, month_r2);
                }

                ViewBag.keyword = keyword;
                ViewBag.data = bks.ViewOfMonthApi(keyword);
                return PartialView("_Partial_Data_Month_New");
            }
            else
            {

                ViewBag.keyword = keyword;
                ViewBag.data = bks.SearchBookingApi(keyword);     
                return PartialView("_Partial_Data_Search_New");
            }
        }

        //[HttpGet]
        //public IActionResult Booking()
        //{
        //    ViewBag.id = HttpContext.Request.Query["Id"];
        //    ViewBag.bookType = HttpContext.Request.Query["BookType"];
        //    ViewBag.num_person = int.Parse(HttpContext.Request.Query["Number_Person"]);
        //    ViewBag.list_airport = bks.List_Airports();
        //    return View();
        //}

        public IActionResult Booking(string jsonString)
        {
            dynamic obj = JsonConvert.DeserializeObject(jsonString);
            string id = obj.Id;
            Keywords keyword = new Keywords();
            keyword.BookType = obj.BookType;
            keyword.Adult = int.Parse(obj.adult.ToString());
            keyword.Children = obj.child != null ? int.Parse(obj.child.ToString()) : 0;
            keyword.Baby = obj.baby != null ? int.Parse(obj.baby.ToString()) : 0;
            keyword.NumberOfPassengers = keyword.Adult + keyword.Children + keyword.Baby;
            List<Models.GroupBooking.Result> flights = bks.GetDataById(id).Result;
            List<Models.GroupBooking.Result> q = new List<Models.GroupBooking.Result>();
            foreach (var item in flights)
            {
                item.typeOfTrip = obj.BookType;
                item.numberOfGuests = keyword.NumberOfPassengers;
                item.departure = obj.Departure;
                item.arrival = obj.Arrival;
                q.Add(item);
            }
            ViewBag.filghts = q;
            ViewBag.keyword = keyword;
            return PartialView("_Partial_Booking");
        }

        public async Task<string> SaveBooking(InfoBooking info)
        {
            using (var db = new TicketContext())
            {
                InfoBooking _info = new InfoBooking();
                List<Flight> flights = new List<Flight>();

                List<Models.GroupBooking.Result> f = bks.GetDataById(info.ID).Result;

                _info.ID = info.ID;
                _info.AgentCode = "NV00376";
                _info.BookType = info.BookType;
                _info.Phone = info.Phone;
                _info.PhoneRemark = info.PhoneRemark;
                _info.Email = info.Email;
                _info.Remark = info.Remark;
                _info.NumberOfPassengers = info.NumberOfPassengers;
                _info.Fare = 0;
                _info.Charge = 0;
                _info.Price = f[0].price;
                _info.Passengers = info.Passengers;

                //Route 1
                Flight r1 = new Flight();
                r1.RouteNo = 1;
                r1.DepartureCode = f[0].routes[0].departureCode.Trim();
                r1.ArrivalCode = f[0].routes[0].arrivalCode.Trim();
                r1.FlightCode = f[0].routes[0].flightCode.Trim();
                r1.DepartureDate = f[0].routes[0].departureDate;
                r1.AirlineSystem = f[0].airline;
                r1.FlightAirline = f[0].routes[0].flightCode.Substring(0, 2);
                flights.Add(r1);

                if (info.BookType == "ROUNDTRIP")
                {
                    //Route 2
                    Flight r2 = new Flight();
                    r2.RouteNo = 2;
                    r2.DepartureCode = f[0].routes[1].departureCode.Trim();
                    r2.ArrivalCode = f[0].routes[1].arrivalCode.Trim();
                    r2.FlightCode = f[0].routes[1].flightCode.Trim();
                    r2.DepartureDate = f[0].routes[1].departureDate;
                    r2.AirlineSystem = f[0].airline;
                    r2.FlightAirline = f[0].routes[1].flightCode.Substring(0, 2);
                    flights.Add(r2);
                }
                _info.Flights = flights;

                return await bks.InsertBookings(_info, token);
            }
        }

        public JsonResult CreateBooking(string data)
        {
            Dictionary<string,dynamic> obj = JsonConvert.DeserializeObject<Dictionary<string,dynamic>>(data);
            InfoBooking infoBooking = new InfoBooking();
            infoBooking.ID = obj["Id"];
            infoBooking.Remark = obj["Remark"];
            infoBooking.Phone = obj["Phone"];
            infoBooking.Email = obj["Email"];
            infoBooking.BookType = obj["BookType"];
            infoBooking.NumberOfPassengers = int.Parse(obj["Person"].ToString());
            List<Passenger> listPassenger = new List<Passenger>();
            for (int i = 1; i <= infoBooking.NumberOfPassengers; i++)
            {
                Passenger passenger = new Passenger();
                passenger.FullName = obj["fullname" + i.ToString()];
                passenger.Title = obj["title" + i.ToString()];
                passenger.DateOfBirth = obj["dateOfBirth" + i.ToString()].ToString() != "" ? ExtensionHelper.Format_DateTime(obj["dateOfBirth" + i.ToString()].ToString()) : null;
                listPassenger.Add(passenger);
            }
            infoBooking.Passengers = listPassenger;
            string result = SaveBooking(infoBooking).Result;
            return Json(new
            {
                msg = result
            });
        }

        //public async Task<List<EVCBooking.Models.GroupBooking.Result>> GetDataById()
        //{
        //    return await bks.GetDataById("0oVOFzy0K5c=");
        //}


        public Object List_Json_Airports()
        {
            return bks.List_Json_Airports();
        }

        public IActionResult Popular_Airports()
        {
            var result = bks.Popular_Airports();
            var json = JsonConvert.SerializeObject(result);
            return Json(new
            {
                result = json
            });
        }

        //public IActionResult List_Airports(string keyword)
        //{
        //    var result = bks.List_Airports(keyword);
        //    var json = JsonConvert.SerializeObject(result);
        //    return Json(new
        //    {
        //        result = json
        //    });
        //}

        public string Airport()
        {
            string str = ExtensionHelper.Format_Airport("Ho Chi Minh (SGN)");
            return str;
        }
 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}