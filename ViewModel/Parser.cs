using AngleSharp;
using AngleSharp.Dom;
using CourseWork.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI; 

namespace CourseWork.ViewModel
{
    public class Parser(string URL)
    {
        public string MainURL = URL;

        public async Task<List<string>> GetLinks()
        {
            List<string> links = [];
            string LinkNextPage = MainURL;

            using (IWebDriver driver = new EdgeDriver())
            {
                while (LinkNextPage != "https://www.tripadvisor.ru")
                {
                    driver.Navigate().GoToUrl(LinkNextPage);

                    var aElements = driver.FindElements(By.CssSelector("div.WTWEM.w._Z > a"));

                    foreach (var a in aElements.ToList())
                    {
                        links.Add($"https://www.tripadvisor.ru{a.GetDomAttribute("href")}");
                    }

                    var aNextPage = driver.FindElement(By.CssSelector("div.lyTpA.j > div.qyYEI > div > a"));
                    LinkNextPage = "https://www.tripadvisor.ru" + (aNextPage is not null ? aNextPage.GetDomAttribute("href") : "");
                }
            }
            return links;
        }

        public static async Task<ParserData> Parse(string url)
        {
            ParserData hotel;

            using (IWebDriver driver = new EdgeDriver())
            {
                driver.Navigate ().GoToUrl(url);

                //-------------------------------Основная Инфа---------------------



                var hotelName = driver.FindElement(By.CssSelector("h1.biGQs._P.rRtyp"))?.Text;
                var address = driver.FindElement(By.CssSelector("span.fRIUK.CdhWK._S > span.biGQs._P.pZUbB.KxBGd"))?.Text;
                var rating = driver.FindElement(By.CssSelector("div.dGsKv.Xe.f.P > button > span > div.jVDab.W.f.u.w.GOdjs > svg.UctUV.d.H0.hzzSG > title"))?.Text.Split(" ")[0];
                var reviewCount = driver.FindElement(By.CssSelector("div.hvAtG > a > div > span"))?.Text.Split(" ")[0];

                var ratingAdds = driver.FindElements(By.CssSelector("div.ZuFMR.f > svg > title"));
                double?[] Ratings = new double?[6];
                double?[] sourceArray = ratingAdds.Select(p => (double?)double.Parse(p.Text.Split(" ")[0])).ToArray(); //
                Array.Copy(sourceArray, Ratings, Math.Min(sourceArray.Length, Ratings.Length));

                //---------------------------------Удобства----------

                var Amenities = driver.FindElements(By.CssSelector("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(2) > div.gFttI.f.ME.Ci.H3._c > span"));
                var AmenitiesAdds = driver.FindElements(By.CssSelector("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(2) > div.XfIwF.f.K.w.KEDvW > div.gFttI.f.ME.Ci.H3._c > span"));

                List<string> AmenitiesList = [];
                AmenitiesList = Amenities.Select(p => p.Text).ToList();

                foreach (var Amenity in AmenitiesAdds)
                {
                    AmenitiesList.Add(Amenity.Text);
                }


                var Facilities = driver.FindElements(By.CssSelector("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(5) > div.gFttI.f.ME.Ci.H3._c > span"));
                var FacilitiesAdds = driver.FindElements(By.CssSelector("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(5) > div.XfIwF.f.K.w.KEDvW > div.gFttI.f.ME.Ci.H3._c > span"));

                List<string> FacilitiesList = [];
                FacilitiesList = Facilities.Select(p => p.Text).ToList();

                foreach (var Facility in FacilitiesAdds)
                {
                    FacilitiesList.Add(Facility.Text);
                }


                hotel = new()
                {
                    HotelName = hotelName ?? " ",
                    HotelAdress = address ?? " ",
                    HotelRating = (rating is null) ? null : double.Parse(rating),
                    HotelReviewCount = (reviewCount is null) ? null : int.Parse(reviewCount),
                    HotetRatingAdds = Ratings,
                    Amenities = AmenitiesList,
                    Facilities = FacilitiesList,
                };



                //-------------------------------Комменты--------
                string LinkNextPage = url;

                while (LinkNextPage != "https://www.tripadvisor.ru")
                {
                    driver.Navigate().GoToUrl(LinkNextPage);
                    var Reviews = driver.FindElements(By.CssSelector("div.azLzJ.MI.R2.Gi.z.Z.BB.kYVoW.tpnRZ")); //

                    int i = 0;

                    Thread.Sleep(10000);

                    var reviews = driver.FindElements(By.CssSelector("div[class*=\"uqMDf\"] div[class*=\"azLzJ\"]"));
                    foreach (var review in reviews)
                    {
                        string userName = review.FindElement(By.CssSelector("div.tFTbB > div > a > span")).Text;

                        string cityElement = review.FindElement(By.CssSelector("div.tFTbB > span:nth - child(2)")).Text;
                        string? city = cityElement != null && cityElement.Contains(',') ? cityElement : null;

                        string Title = review.FindElement(By.CssSelector("span.JbGkU.Cj > span")).Text;

                        double Rating = double.Parse(review.FindElement(By.CssSelector("div.WcRsW.f.O > div > svg.UctUV.d.H0 > title")).Text.Split(" ")[0]);

                        List<string> DatePostedList = review.FindElement(By.CssSelector("div.tFTbB > div")).Text.Split(" ").ToList();

                        List<string> DateCheckInList = review.FindElement(By.CssSelector("div.PDZqu > span.iSNGb._R.Me.S4.H3.Cj")).Text.Split(" ").ToList();

                        i++;

                        hotel.AddReview(Title, userName, city, Rating, DatePostedList[DatePostedList.Count - 3], int.Parse(DatePostedList[DatePostedList.Count) - 2]),
                                        DateCheckInList[DateCheckInList.Count - 3], int.Parse(DateCheckInList[DateCheckInList.Count - 2]));
                        //Console.WriteLine($"{i}, {city}, {userName}, {Title}, {Rating}, {DatePosted}, {DateCheckIn}");

                        Random random = new Random();
                        Thread.Sleep(new Random().Next(2000, 5000));
                    }



                    var aNextPage = driver.FindElement(By.CssSelector("a.BrOJk.u.j.z._F.wSSLS.tIqAi.unMkR"));
                    LinkNextPage = "https://www.tripadvisor.ru" + (aNextPage is not null ? aNextPage.GetDomAttribute("href") : "");
                }
            }

            return hotel;
        }
    }
}


//public class Parser(string URL)
//{
//    public string MainURL = URL;

//    public async Task<List<string>> GetLinks()
//    {
//        IConfiguration config = Configuration.Default.WithDefaultLoader();
//        IBrowsingContext context = BrowsingContext.New(config);

//        List<string> links = [];
//        string LinkNextPage = MainURL;

//        while (LinkNextPage != "https://www.tripadvisor.ru")
//        {
//            IDocument doc = await context.OpenAsync(LinkNextPage);
//            var aElements = doc.QuerySelectorAll("div.WTWEM.w._Z > a");

//            foreach (IElement a in aElements.ToList())
//            {
//                links.Add($"https://www.tripadvisor.ru{a.GetAttribute("href")}");
//            }

//            var aNextPage = doc.QuerySelector("div.lyTpA.j > div.qyYEI > div > a");
//            LinkNextPage = "https://www.tripadvisor.ru" + (aNextPage is not null ? aNextPage.GetAttribute("href") : "");
//        }

//        return links;
//    }

//    public static async Task<ParserData> Parse(string url)
//    {
//        IConfiguration config = Configuration.Default.WithDefaultLoader();
//        IBrowsingContext context = BrowsingContext.New(config);
//        IDocument doc = await context.OpenAsync(url);

//        //-------------------------------Основная Инфа---------------------

//        IElement hotelName = doc.QuerySelector("h1.biGQs._P.rRtyp")!;
//        string? HotelName = hotelName.TextContent;

//        IElement address = doc.QuerySelector("span.fRIUK.CdhWK._S > span.biGQs._P.pZUbB.KxBGd")!;
//        string Address = address.TextContent;

//        IElement? rating = doc.QuerySelector("div.dGsKv.Xe.f.P > button > span > div.jVDab.W.f.u.w.GOdjs > svg.UctUV.d.H0.hzzSG > title")!;
//        string? Rating = rating?.TextContent.Split(" ")[0]; //

//        IElement? reviewCount = doc.QuerySelector("div.hvAtG > a > div > span")!;
//        string? ReviewCount = reviewCount?.TextContent.Split(" ")[0]; //


//        var ratingAdds = doc.QuerySelectorAll("div.ZuFMR.f > svg > title")!;


//        double?[] Ratings = new double?[6];
//        double?[] sourceArray = ratingAdds.Select(p => (double?)double.Parse(p.TextContent.Split(" ")[0])).ToArray(); //
//        Array.Copy(sourceArray, Ratings, Math.Min(sourceArray.Length, Ratings.Length));

//        //---------------------------------Удобства----------

//        IHtmlCollection<IElement> Amenities = doc.QuerySelectorAll("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(2) > div.gFttI.f.ME.Ci.H3._c > span");
//        IHtmlCollection<IElement>? AmenitiesAdds = doc.QuerySelectorAll("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(2) > div.XfIwF.f.K.w.KEDvW > div.gFttI.f.ME.Ci.H3._c > span");

//        List<string> AmenitiesList = [];
//        AmenitiesList = Amenities.Select(p => p.TextContent).ToList();

//        foreach (var Amenity in AmenitiesAdds)
//        {
//            AmenitiesList.Add(Amenity.TextContent);
//        }


//        IHtmlCollection<IElement> Facilities = doc.QuerySelectorAll("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(5) > div.gFttI.f.ME.Ci.H3._c > span");
//        IHtmlCollection<IElement>? FacilitiesAdds = doc.QuerySelectorAll("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(5) > div.XfIwF.f.K.w.KEDvW > div.gFttI.f.ME.Ci.H3._c > span");

//        List<string> FacilitiesList = [];
//        FacilitiesList = Facilities.Select(p => p.TextContent).ToList();

//        foreach (var Facility in FacilitiesAdds)
//        {
//            FacilitiesList.Add(Facility.TextContent);
//        }

//        //-------------------------------Комменты--------
//        string LinkNextPage = url;

//        while (LinkNextPage != "https://www.tripadvisor.ru")
//        {
//            IDocument docView = await context.OpenAsync(LinkNextPage);



//            var aElements = docView.QuerySelectorAll("div.WTWEM.w._Z > a");

//            IHtmlCollection<IElement>? Reviews = docView.QuerySelectorAll("div.azLzJ.MI.R2.Gi.z.Z.BB.kYVoW.tpnRZ"); //

//            IEnumerable<IElement> BlockReviews = docView.All.Where(block =>
//                block.LocalName == "div"
//                && block.ClassList.Contains("azLzJ.MI.R2.Gi.z.Z.BB.kYVoW.tpnRZ")
//                );



//            var aNextPage = doc.QuerySelector("a.BrOJk.u.j.z._F.wSSLS.tIqAi.unMkR");
//            LinkNextPage = "https://www.tripadvisor.ru" + (aNextPage is not null ? aNextPage.GetAttribute("href") : "");
//        }


//        ParserData hotel = new()
//        {
//            HotelName = HotelName,
//            HotelAdress = Address,
//            HotelRating = (Rating is null) ? null : double.Parse(Rating),
//            HotelReviewCount = (ReviewCount is null) ? null : int.Parse(ReviewCount),
//            HotetRatingAdds = Ratings,
//            Amenities = AmenitiesList,
//            Facilities = FacilitiesList,
//        };

//        return hotel;
//    }
//}