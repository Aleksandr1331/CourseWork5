using CourseWork.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;

namespace CourseWork.ViewModel
{
    public static class Parser
    {
        public static async Task<List<string>> GetLinksAsync(string mainUrl)
        {
            return await Task.Run(() => GetLinks(mainUrl));
        }

        public static async Task<ParserData> ParseAsync(string url)
        {
            return await Task.Run(() => Parse(url));
        }

        private static EdgeDriver GetEdgeDriver()
        {
            var options = new EdgeOptions();

            //options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
             options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("user-agent=" + GetRandomUserAgent());

            options.AddArgument("user-data-dir=C:\\Users\\Tawori\\AppData\\Local\\Microsoft\\Edge\\User Data");
            //options.AddArgument("user-data-dir=C:\\Users\\Tawori\\AppData\\Roaming\\Opera Software");
            options.AddArgument("profile-directory=Default");
            //options.AddArgument("profile-directory=Opera GX Stable");
            options.AddArgument("--start-maximized");

            string driverPath = @"C:\Users\Tawori\OneDrive\Documents\edgedriver_win64\msedgedriver.exe";
            //string OperaDriverPath = @"C:\Users\Tawori\AppData\Local\Programs\Opera GX\opera.exe";
            var service = EdgeDriverService.CreateDefaultService(driverPath);
            service.EnableVerboseLogging = true;

            return new EdgeDriver(service, options);
        }

        private static string GetRandomUserAgent()
        {
            var userAgents = new List<string>
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
                "Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1"
            };
            Random random = new();
            return userAgents[random.Next(userAgents.Count)];
        }

        public static List<string> GetLinks(string MainURL)
        {
            List<string> links = [];
            string LinkNextPage = MainURL;

            using (var driver = GetEdgeDriver())
            {
                while (LinkNextPage != "https://www.tripadvisor.ru")
                {
                    driver.Navigate().GoToUrl(LinkNextPage);

                    Thread.Sleep(10000);

                    IWebElement? button = driver.FindElements(By.CssSelector("div.hhxIz.f.v.u.j > button.rmyCe._G.B-.z._S.c.Wc")).FirstOrDefault();
                    button?.Click();

                    // Эмуляция пользовательских действий
                    Actions actions = new(driver);
                    actions.MoveByOffset(10, 10).Perform();
                    Thread.Sleep(new Random().Next(3000, 6000));
                    //-------------------------------------

                    var aElements = driver.FindElements(By.CssSelector("div.WTWEM.w._Z > a"));
                    links.AddRange(aElements.Select(a => $"https://www.tripadvisor.ru{a.GetDomAttribute("href")}"));

                    IWebElement? aNextPage = driver.FindElements(By.CssSelector("div.lyTpA.j > div.qyYEI > div > a")).FirstOrDefault();
                    LinkNextPage = "https://www.tripadvisor.ru" + (aNextPage is not null ? aNextPage.GetDomAttribute("href") : "");
                }
            }
            return links;
        }

        public static ParserData Parse(string url)
        {
            ParserData hotel;

            using (var driver = GetEdgeDriver())
            {
                driver.Navigate().GoToUrl(url);

                // Эмуляция пользовательских действий
                Actions actions = new(driver);
                actions.MoveByOffset(10, 10).Perform();
                Thread.Sleep(new Random().Next(5000, 7000));
                //-------------------------------------

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
                var amenitiesButton = driver.FindElements(By.CssSelector("div > div:nth-child(2) > div > button.UikNM")).ElementAtOrDefault(0);
                List<string> AmenitiesList = [];
                if (amenitiesButton != null)
                {
                    amenitiesButton.Click();
                    Thread.Sleep(2000);
                    AmenitiesList = driver.FindElements(By.CssSelector("div.aoEpU > div.YWBVt.K.I.Pf > div"))
                        .Select(A => A.Text)
                        .ToList();
                    driver.FindElement(By.CssSelector("button[data-automation=closeModal]")).Click();
                }
                else
                {
                    AmenitiesList = driver.FindElements(By.CssSelector("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(2) > div.gFttI.f.ME.Ci.H3._c"))
                        .Select(A => A.Text)
                        .ToList();
                }


                var facilitiesButton = driver.FindElements(By.CssSelector("div > div:nth-child(2) > div > button.UikNM")).ElementAtOrDefault(1);
                List<string> FacilitiesList = [];
                if (facilitiesButton != null)
                {
                    facilitiesButton.Click();
                    Thread.Sleep(2000);
                    FacilitiesList = driver.FindElements(By.CssSelector("div.aoEpU > div.YWBVt.K.I.Pf > div"))
                        .Select(F => F.Text)
                        .ToList();
                    driver.FindElement(By.CssSelector("button[data-automation=closeModal]")).Click();
                }
                else
                {
                    FacilitiesList = driver.FindElements(By.CssSelector("#ABOUT_TAB > div.ruCQl.z > div:nth-child(2) > div:nth-child(5) > div.gFttI.f.ME.Ci.H3._c"))
                        .Select(F => F.Text)
                        .ToList();
                }

                //------------Сохранение основных записей-------

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


                //-------------------------------Комментарии--------
                string LinkNextPage = url;

                while (LinkNextPage != "https://www.tripadvisor.ru" && hotel.Reviews.Count < 50)
                {
                    driver.Navigate().GoToUrl(LinkNextPage);

                    Thread.Sleep(new Random().Next(3000, 5000));

                    var reviews = driver.FindElements(By.CssSelector("div[class*=\"uqMDf\"] div[class*=\"azLzJ\"]"));
                    foreach (var review in reviews)
                    {
                        IWebElement? userNameEl = review.FindElements(By.CssSelector("div.tFTbB > div > a > span")).FirstOrDefault();
                        string? userName = userNameEl != null ? userNameEl.Text : string.Empty;

                        IWebElement? cityEl = review.FindElements(By.CssSelector("div.tFTbB > span:nth-child(2)")).FirstOrDefault();
                        string? city = cityEl != null && cityEl.Text.Contains(',') ? cityEl.Text : string.Empty;    ////// Поменять на Null

                        IWebElement? TitleEl = review.FindElements(By.CssSelector("span.JbGkU.Cj > span")).FirstOrDefault();
                        string? Title = TitleEl != null ? TitleEl.Text : string.Empty;

                        Thread.Sleep(1000);

                        IWebElement? RatingEl = review.FindElements(By.CssSelector("div.WcRsW.f.O > div > svg.UctUV.d.H0 > title")).FirstOrDefault();
                        double? Rating = RatingEl != null && double.TryParse(RatingEl.Text.Split(" ")[0], out double RatingDouble) ? RatingDouble : Double.NaN;

                        IWebElement? DatePostedEl = review.FindElements(By.CssSelector("div.tFTbB > div")).FirstOrDefault();
                        List<string>? DatePostedList = DatePostedEl?.Text.Split(" ").ToList();
                        string DatePostedMonth = DatePostedList != null ? DatePostedList[^3] : "Такого не может быть";
                        int DatePostedDay = DatePostedList != null && int.TryParse(DatePostedList[^2], out int PostedDay) ? PostedDay : 1;

                        IWebElement? DateCheckInEl = review.FindElements(By.CssSelector("div.PDZqu > span.iSNGb._R.Me.S4.H3.Cj")).FirstOrDefault();
                        List<string>? DateCheckInList = DateCheckInEl?.Text.Split(" ").ToList();
                        string DateCheckInMonth = DateCheckInList != null ? DateCheckInList[^3] : "Такого не может быть";
                        int DateCheckInDay = DateCheckInList != null && int.TryParse(DateCheckInList[^2], out int CheckInDay) ? CheckInDay : 1;

                        hotel.AddReview(Title, userName, city, Rating, DatePostedMonth,
                                        DatePostedDay, DateCheckInMonth, DateCheckInDay);
                        Thread.Sleep(new Random().Next(1000, 3000));
                    }
                    //actions.MoveByOffset(9, 9).Perform();

                    IWebElement? aNextPage = driver.FindElements(By.CssSelector("div.xkSty > div > a")).FirstOrDefault();
                    LinkNextPage = "https://www.tripadvisor.ru" + (aNextPage is not null ? aNextPage.GetDomAttribute("href") : "");
                }
                Thread.Sleep(new Random().Next(1000, 3000));
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

//        //-------------------------------Комментарии--------
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
//            HotelAddress = Address,
//            HotelRating = (Rating is null) ? null : double.Parse(Rating),
//            HotelReviewCount = (ReviewCount is null) ? null : int.Parse(ReviewCount),
//            HotelRatingAdds = Ratings,
//            Amenities = AmenitiesList,
//            Facilities = FacilitiesList,
//        };

//        return hotel;
//    }
//}