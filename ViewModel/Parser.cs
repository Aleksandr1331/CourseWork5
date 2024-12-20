using CourseWork.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace CourseWork.ViewModel
{
    public static class Parser
    {
        public static async Task<List<string>> GetLinksAsync(string mainUrl, int count)
        {
            return await Task.Run(() => GetLinks(mainUrl, count));
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
            options.AddExcludedArgument("enable-automation");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-features=IsolateOrigins,site-per-process");

            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            options.AddArgument("user-data-dir=C:\\Users\\Tawori\\AppData\\Local\\Microsoft\\Edge\\User Data");
            options.AddArgument("profile-directory=Default");
            options.AddArgument("--start-maximized");

            string driverPath = @"C:\Users\Tawori\OneDrive\Documents\edgedriver_win64\msedgedriver.exe";
            var service = EdgeDriverService.CreateDefaultService(driverPath);
            service.EnableVerboseLogging = true;

            return new EdgeDriver(service, options);
        }

        public static List<string> GetLinks(string MainURL, int Count)
        {
            List<string> links = [];
            string LinkNextPage = MainURL;

            using (var driver = GetEdgeDriver())
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                while (LinkNextPage != "https://2droida.ru/catalog/smartfony?page=NaN" && links.Count < Count)
                {
                    driver.Navigate().GoToUrl(LinkNextPage);
                    wait.Until(driver => driver.FindElement(By.CssSelector("div.product-name.font-adaptive > a")).Displayed);

                    var aElements = driver.FindElements(By.CssSelector("div.product-name.font-adaptive > a"));
                    links.AddRange(aElements.Select(a => $"https://2droida.ru{a.GetDomAttribute("href")}"));

                    IWebElement? aNextPage = driver.FindElements(By.CssSelector("div.pagination-area > nav > div.page-item > a")).FirstOrDefault(a => a.Text.Trim() == "Следующая");
                    LinkNextPage = "https://2droida.ru" + (aNextPage is not null ? aNextPage.GetDomAttribute("href") : "");
                }
            }
            return links.GetRange(0, Count);
        }

        public static ParserData Parse(string url)
        {
            ParserData smartphone;

            using (var driver = GetEdgeDriver())
            {
                driver.Navigate().GoToUrl(url);
                var actions = new Actions(driver);
                //-------------------------------Основная Инфа---------------------

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElement(By.CssSelector("div button.btn.text-primary")).Displayed);
                Thread.Sleep(5000);

                IWebElement? button = driver.FindElements(By.CssSelector("div button.btn.text-primary")).FirstOrDefault();
                actions.MoveToElement(button).Perform();
                button?.Click();

                wait.Until(driver => driver.FindElement(By.CssSelector("tbody[data-v-ad40dac3]")).Displayed);
                var rows = driver.FindElements(By.CssSelector("tbody[data-v-ad40dac3] tr"));

                Dictionary<string, string> table = [];
                foreach (var row in rows)
                {
                    var header = row.FindElement(By.CssSelector("th")).Text;
                    var value = row.FindElement(By.CssSelector("td > p")).Text;
                    table.Add(header, value);
                }

                string brand = table["Бренд"];
                string model = table["Модель"];
                string color = table["Цвет"];
                string matrixType = table["Тип матрицы экрана"] ?? table["Технология экрана"];
                double screenDiagonal = double.Parse(table["Диагональ экрана"].Replace(".", ","));
                int batteryCapacity = int.Parse(table["Емкость аккумулятора"].Split(" ")[0]);
                int rEM = int.Parse(table["Оперативная память"].Split(" ")[0]);

                //---------------------------------Цена----------
                double currentPrice = double.TryParse(driver.FindElements(By.CssSelector("span.current-price span")).FirstOrDefault()?.Text.Replace(".", ","), out double CurPrice) ? CurPrice : 0;

                IWebElement? discountEl = driver.FindElements(By.CssSelector("span.hot")).FirstOrDefault();
                int? discount = null;
                double? oldPrice = null;

                if (discountEl != null)
                {
                    discount = int.TryParse(discountEl.Text.Replace("%", ""), out int disc) ? disc : null;
                    oldPrice = double.TryParse(driver.FindElements(By.CssSelector("span.old-price span")).FirstOrDefault()?.Text.Replace(".", ","), out double OPrice) ? OPrice : null;
                }

                List<string> equipment = table["Подробная комплектация"].Split(", ").ToList();

                List<string> cameraType = table["Тип основных камер"].Split(", ").ToList();
                List<string> cameraSpecs = table
                    .Where(t => t.Key.StartsWith("Характеристики основной камеры"))
                    .Select(t => t.Value)
                    .ToList();

                var tagsEl = driver.FindElements(By.CssSelector("div.product-tags a"));
                List<string> tags = [];

                if (tagsEl != null)
                    tags = tagsEl.Select(t => t.Text).ToList();



                //------------Сохранение основных записей-------

                smartphone = new()
                {
                    Brand = brand,
                    Model = model,
                    Color = color,
                    CurrentPrice = currentPrice,
                    Dicsount = discount,
                    OldPrice = oldPrice,
                    MatrixType = matrixType,
                    Screen_Diagonal = screenDiagonal,
                    BatteryCapacity = batteryCapacity,
                    REM = rEM,

                    Equipment = equipment,
                    Tags = tags
                };

                //-------------------------------Камеры--------
                foreach ((string First, string Second) in cameraType.Zip(cameraSpecs).ToList())
                    smartphone.AddCamera(First, Second);
            }
            return smartphone;
        }
    }
}