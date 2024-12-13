namespace CourseWork.Model
{
    public class ParserData
    {
        public string HotelName { get; set; } = string.Empty;
        public string HotelAdress { get; set; } = string.Empty;
        public double? HotelRating { get; set; } = null;
        public int? HotelReviewCount { get; set; } = 0;
        public double?[] HotetRatingAdds { get; set; } = [null, null, null, null, null, null];

        public List<string> Amenities { get; set; } = [];
        public List<string> Facilities { get; set; } = [];

        public List<ReviewData> Reviews { get; set; } = [];


        public void AddReview(string title, string userName, string city,
                              double? rating, string datePostMonth, int datePostDay,
                              string dateCheckInMonth, int dateCheckInDay)
        {
            ReviewData reviewData = new(title, userName, city, rating, datePostMonth, datePostDay, dateCheckInMonth, dateCheckInDay);
            Reviews.Add(reviewData);
        }
    }

    public class ReviewData
    {
        private readonly Dictionary<string, int> GetMonth = new()
        {
            { "январь", 1 }, { "янв.", 1 },
            { "февраль", 2 }, { "февр.", 2 },
            { "март", 3 },
            { "апрель", 4 }, { "апр.", 4 },
            { "май", 5 },
            { "июнь", 6 },
            { "июль", 7 },
            { "август", 8 }, { "авг.", 8 },
            { "сентябрь", 9 }, { "сент.", 9 },
            { "октябрь", 10 }, { "окт.", 10 },
            { "ноябрь", 11 }, { "нояб.", 11 },
            { "декабрь", 12 }, { "дек.", 12 },
        };

        public string Title { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double? Rating { get; set; } = null;
        public DateOnly DatePost { get; set; } = DateOnly.MinValue;
        public DateOnly DateCheckIn { get; set; } = DateOnly.MinValue;

        public ReviewData(string title, string userName, string city,
                          double? rating, string datePostMonth, int datePostDay,
                          string dateCheckInMonth, int dateCheckInDay)
        {
            Title = title;
            UserName = userName;
            Rating = rating;
            City = city;
            DatePost = GetData(datePostMonth, datePostDay);
            DateCheckIn = GetData(dateCheckInMonth, dateCheckInDay);
        }

        private DateOnly GetData(string DateMonth, int DateYear)
        {
            int day = 1;
            int month = GetMonth[DateMonth.Trim()];
            int year = DateYear;
            DateOnly dateOnly = new DateOnly(year, month, day);
            
            return dateOnly;
        }
    }
}