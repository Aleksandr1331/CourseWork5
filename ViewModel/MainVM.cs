using CourseWork.Commands;
using CourseWork.Model;
using CourseWork.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CourseWork.ViewModel
{
    public class MainVM
    {
        public ObservableCollection<ParserData> Dates { get; set; } = [];
        public ICommand StartParsingCommand { get; set; }


        public MainVM()
        {
            StartParsingCommand = new RelayCommand(async () => await GetLinksAsync());
        }


        private async Task GetLinksAsync()
        {
            List<string> links = await Parser.GetLinksAsync("https://www.tripadvisor.ru/Hotels-g665310-Tomsk_Tomsk_Oblast_Siberian_District-Hotels.html");
            Dates.Clear();

            foreach (string link in links)
            {
                ParserData data = await Parser.ParseAsync(link);
                Dates.Add(data);

                await DataToBD(data);
            }
        }


        private static async Task DataToBD(ParserData data)
        {
            using (HotelsContext db = new())
            {
                //Создание отеля
                Hotel hotel = new Hotel
                {
                    Name = data.HotelName,
                    Adress = data.HotelAdress,
                    Rating = data.HotelRating,
                    ReviewCount = data.HotelReviewCount,
                    RatingLocation = data.HotetRatingAdds.ElementAtOrDefault(0),
                    RatingRooms = data.HotetRatingAdds.ElementAtOrDefault(1),
                    RatingPriceQuality = data.HotetRatingAdds.ElementAtOrDefault(2),
                    RatingPurity = data.HotetRatingAdds.ElementAtOrDefault(3),
                    RatingService = data.HotetRatingAdds.ElementAtOrDefault(4),
                    RatingSleepQuality = data.HotetRatingAdds.ElementAtOrDefault(5),
                    Amenities = [],
                    Facilities = [],
                    Reviews = [],
                };
                db.Hotels.Add(hotel);

                // Обработка удобств
                foreach (var amenityName in data.Amenities)
                {
                    var amenity = db.Amenities.FirstOrDefault(a => a.AmenityName == amenityName)
                                ?? new Amenity { AmenityName = amenityName, Hotels = new List<Hotel>() };

                    if (!amenity.Hotels.Contains(hotel))
                        amenity.Hotels.Add(hotel);

                    hotel.Amenities.Add(amenity);

                    if (amenity.AmenityId == Guid.Empty)
                        db.Amenities.Add(amenity);
                }

                //Обработка вторых удобств
                foreach (var facilityName in data.Facilities)
                {
                    var facility = db.Facilities.FirstOrDefault(f => f.FacilitiesName == facilityName)
                                   ?? new Facility { FacilitiesName = facilityName, Hotels = [] };

                    if (!facility.Hotels.Contains(hotel))
                        facility.Hotels.Add(hotel);

                    hotel.Facilities.Add(facility);

                    if (facility.FacilitiesId == Guid.Empty)
                        db.Facilities.Add(facility);
                }

                // Обработка отзывов
                foreach (ReviewData reviewData in data.Reviews)
                {
                    //Город
                    City city = db.Cities.FirstOrDefault(c => c.CityName == reviewData.City)
                        ?? new City { CityName = reviewData.City, Users = [] };

                    if (city.CityId == Guid.Empty)
                        db.Cities.Add(city);

                    //Пользователь
                    User user = db.Users.FirstOrDefault(u => u.UserName == reviewData.UserName)
                        ?? new User { UserName = reviewData.UserName, City = city, Reviews = new List<Review>() };

                    if (user.UserId == Guid.Empty)
                        db.Users.Add(user);

                    if (!city.Users.Contains(user))
                        city.Users.Add(user);

                    //Отзыв
                    Review NewReview = new Review
                    {
                        Title = reviewData.Title,
                        User = user,
                        Hotel = hotel,
                        Rating = reviewData.Rating,
                        DatePosted = reviewData.DatePost,
                        DateCheckIn = reviewData.DateCheckIn
                    };


                    //Связь
                    hotel.Reviews.Add(NewReview);
                    user.Reviews.Add(NewReview);
                    db.Reviews.Add(NewReview);

                    db.Add(NewReview);
                }

                await db.SaveChangesAsync();
            }
        }
    }

    //public class BasicVM : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler? PropertyChanged;
    //    protected void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
}
