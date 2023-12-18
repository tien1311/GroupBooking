namespace EVCBooking.Models.Booking
{
    public class Passenger
    {
        private string fullName;
        private string GetFirstName()
        {
            // Tách và lấy phần tử đầu tiên từ FullName
            string[] nameParts = fullName?.Split(' ') ?? new string[0];
            return nameParts.Length > 0 ? nameParts[0] : string.Empty;
        }

        private string GetLastName()
        {
            // Kết hợp các phần tử còn lại từ FullName (sau phần tử đầu tiên)
            string[] nameParts = fullName?.Split(' ') ?? new string[0];
            return nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;
        }

        public string Type { get; set; }
        public string Title { get; set; }
        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                // Cập nhật FirstName và LastName khi FullName thay đổi
                FirstName = GetFirstName();
                LastName = GetLastName();
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<Document> Documents { get; set; }
    }

}
