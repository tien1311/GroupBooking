using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EVCBooking.Services
{
    public class ExtensionHelper
    {
        //Hàm format datetime phù hợp chuẩn việt nam
        public static DateTime Format_DateTime(string dateString)
        {
            //string dateString = "17-11-2023";
            DateTime dateTime;       
    
            if (DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                // Chuyển đổi thành công
                return dateTime;
            }
            else
            {
                // Ví dụ 2: Ném một ngoại lệ tùy ý
                throw new FormatException("Chuyển đổi không thành công.");
            }
        }

        public static string Format_Airport(string airportString) 
        {
            string input = airportString.Split(" ")[airportString.Split(" ").Length - 1];
            string pattern = "[()!@#$%]";
            string result = Regex.Replace(input, pattern, string.Empty);
            return result;
        }

        public static string Format_Number(double  number)
        {
            double output = 0.0;
            if (number > 1000)
            {
                output = number / 1000;
            }
            return $"{output/1000:F3}K";
        }

        public static string GenerateColorFromNumber(int number)
        {
            // Sử dụng hàm hash để tạo giá trị số nguyên từ số
            int hashCode = number.GetHashCode();

            // Chuyển giá trị số nguyên thành màu sắc hex
            string hexColor = (hashCode & 0x00FFFFFF).ToString("X6");

            // Đảm bảo chuỗi hexColor có đúng 6 ký tự bằng cách thêm 0 ở đầu nếu cần
            hexColor = hexColor.PadLeft(6, '0');

            return $"#{hexColor}";
        }

        public static string RemoveDiacriticsAndConvertToUppercase(string input)
        {
            // Loại bỏ dấu và chuyển đổi thành chữ hoa
            string normalizedString = RemoveDiacritics(input).ToUpperInvariant();

            return normalizedString;
        }

        public static string RemoveDiacritics(string input)
        {
            string decomposed = input.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();

            foreach (char c in decomposed)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }
}
