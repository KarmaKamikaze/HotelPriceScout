using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    [Collection("E2E")]
    public class DashboardPageE2ETests
    {
        private readonly ITestOutputHelper _output; // TODO: Remove output and constructor

        public DashboardPageE2ETests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void DashboardPageShouldRedirectToDashboardWhenHomeLogoIsPressed()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement homeLogo = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-logo")));
            homeLogo.Click();

            string expectedUrl = "https://localhost:5001/Dashboard";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageShouldRedirectToSettingsWhenSettingsNavLinkIsPressed()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement settingsNav = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-settings")));
            settingsNav.Click();

            string expectedUrl = "https://localhost:5001/Settings";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageShouldContainCurrentMonthAndYearWhenOpened()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string expectedMonthAndYear = DateTime.Today.ToString("MMMM yyyy");
            string actualMonthAndYear = wait.Until(webDriver => webDriver.FindElement(By.ClassName("H1MonthAndYear"))).Text;
            
            Assert.Equal(expectedMonthAndYear, actualMonthAndYear);
            Teardown(driver);
        }

        [Fact]
        public void DashboardPageShouldContainOneAdultSelectedDefault()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string expectedNumberOfAdults = "1 adult";
            string actualNumberOfAdults =
                wait.Until(webDriver => webDriver.FindElement(By.Id("number-of-adults"))).GetDomAttribute("placeholder");
            
            Assert.Equal(expectedNumberOfAdults, actualNumberOfAdults);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageShouldContainRoomTypesForOneTwoAndFourAdults()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            string roomTypeOptions =
                wait.Until(webDriver => webDriver.FindElement(By.Id("number-of-adults"))).Text;
            bool containsValidOptions = roomTypeOptions.Split('\n').Length == 3 && 
                                        roomTypeOptions.Contains("1 adult") && 
                                        roomTypeOptions.Contains("2 adults") &&
                                        roomTypeOptions.Contains("4 adults");
            
            Assert.True(containsValidOptions);
            Teardown(driver);
        }

        [Fact]
        public void DashboardPageShouldContainDefaultPredefinedListOfHotelsForFiltering()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            bool containsValidOptions = hotelFilters.All(options => 
                options.Text is "All" or "Local" or "No budget" or "Cabinn Aalborg" or "Kompas Hotel Aalborg" or
                    "Slotshotellet Aalborg" or "Aalborg Airport Hotel" or "Comwell Hvide Hus Aalborg" or
                    "Helnan Phønix Hotel" or "Hotel Jomfru Ane" or "Hotel Scheelsminde" or "Milling Hotel Aalborg" or
                    "Prinsen Hotel" or "Radisson Blu Limfjord Hotel Aalborg" or "Room Rent Prinsen" or
                    "Scandic Aalborg City" or "Scandic Aalborg Øst" or "Zleep Hotel Aalborg");
            bool isDistinctAndContainsAll = hotelFilters.Distinct().Count() == 18 && containsValidOptions;
            
            Assert.True(isDistinctAndContainsAll);
            Teardown(driver);
        }

        [Fact]
        public void DashboardPageShouldContainEveryWeekday()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            Thread.Sleep(1000); // wait to ensure all weekdays are displayed
            ReadOnlyCollection<IWebElement> listOfWeekdays =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("Weekdays")));
            bool allWeekdays = listOfWeekdays.All(day =>
                day.Text is "Monday" or "Tuesday" or "Wednesday" or "Thursday" or "Friday" or "Saturday" or "Sunday");
            bool isDistinctAndContainsAll = listOfWeekdays.Distinct().Count() == 7 && allWeekdays;

            Assert.True(isDistinctAndContainsAll);
            Teardown(driver);
        }

        [Fact]
        public void DashboardPageShouldHighlightCurrentDateInCalendar()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string expectedHighlightDate = DateTime.Today.Day.ToString();
            string actualHighlightedDate = wait.Until(webDriver => webDriver.FindElement(By.ClassName("bg-warning")))
                .Text.Split('\n')[0];

            Assert.Equal(expectedHighlightDate, actualHighlightedDate);
            Teardown(driver);
        }

        private static IWebDriver Setup()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(webDriver => webDriver.FindElement(By.Id("start-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("go-to-dashboard-button"))).Click();

            return driver;
        }

        private static void Teardown(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }
    }
}