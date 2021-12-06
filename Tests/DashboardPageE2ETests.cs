using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Bunit.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Tests
{
    [Collection("E2E")]
    public class DashboardPageE2ETests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        
        public DashboardPageE2ETests()
        { 
            _driver = Setup();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void DashboardPageShouldRedirectToDashboardWhenHomeLogoIsPressed()
        {
            IWebElement homeLogo = _wait.Until(webDriver => webDriver.FindElement(By.Id("nav-logo")));
            homeLogo.Click();

            string expectedUrl = "https://localhost:5001/Dashboard";
            bool urlRedirects = _wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
        }
        
        [Fact]
        public void DashboardPageShouldRedirectToSettingsWhenSettingsNavLinkIsPressed()
        {
            IWebElement settingsNav = _wait.Until(webDriver => webDriver.FindElement(By.Id("nav-settings")));
            settingsNav.Click();

            string expectedUrl = "https://localhost:5001/Settings";
            bool urlRedirects = _wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
        }
        
        [Fact]
        public void DashboardPageShouldContainCurrentMonthAndYearWhenOpened()
        {
            string expectedMonthAndYear = DateTime.Today.ToString("MMMM yyyy", new CultureInfo("en-US"));
            string actualMonthAndYear = _wait.Until(webDriver => webDriver.FindElement(By.ClassName("H1MonthAndYear"))).Text;
            
            Assert.Equal(expectedMonthAndYear, actualMonthAndYear);
        }

        [Fact]
        public void DashboardPageShouldContainOneAdultSelectedDefault()
        {
            string expectedNumberOfAdults = "1 adult";
            string actualNumberOfAdults =
                _wait.Until(webDriver => webDriver.FindElement(By.Id("number-of-adults"))).GetDomAttribute("placeholder");
            
            Assert.Equal(expectedNumberOfAdults, actualNumberOfAdults);
        }
        
        [Fact]
        public void DashboardPageShouldContainRoomTypesForOneTwoAndFourAdults()
        {
            string roomTypeOptions =
                _wait.Until(webDriver => webDriver.FindElement(By.Id("number-of-adults"))).Text;
            bool containsValidOptions = roomTypeOptions.Split('\n').Length == 3 && 
                                        roomTypeOptions.Contains("1 adult") && 
                                        roomTypeOptions.Contains("2 adults") &&
                                        roomTypeOptions.Contains("4 adults");
            
            Assert.True(containsValidOptions);
        }

        [Fact]
        public void DashboardPageShouldContainDefaultPredefinedListOfHotelsForFiltering()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            bool containsValidOptions = hotelFilters.All(options => 
                options.Text is "All" or "Local" or "No budget" or "Cabinn Aalborg" or "Kompas Hotel Aalborg" or
                    "Slotshotellet Aalborg" or "Aalborg Airport Hotel" or "Comwell Hvide Hus Aalborg" or
                    "Helnan Phønix Hotel" or "Hotel Jomfru Ane" or "Hotel Scheelsminde" or "Milling Hotel Aalborg" or
                    "Prinsen Hotel" or "Radisson Blu Limfjord Hotel Aalborg" or "Room Rent Prinsen" or
                    "Scandic Aalborg City" or "Scandic Aalborg Øst" or "Zleep Hotel Aalborg");
            bool isDistinctAndContainsAll = hotelFilters.Distinct().Count() == 18 && containsValidOptions;
            
            Assert.True(isDistinctAndContainsAll);
        }

        [Fact]
        public void DashboardPageFilterDropdownShouldSelectAllHotelsIfAllOptionsIsPressed()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "All").Click();
            Thread.Sleep(1000); // wait to ensure all options are selected
            hotelFilters = _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            bool allOptionsSelected = hotelFilters.All(option => option.GetAttribute("class").Contains("selected"));
            
            Assert.True(allOptionsSelected);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldSelectLocalHotelsIfLocalOptionIsPressed()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "Local").Click();
            Thread.Sleep(1000); // wait to ensure local options are selected
            hotelFilters = _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> selectedFilters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool localSelected = selectedFilters.All(option =>
                option.Text is "Local" or "Cabinn Aalborg" or "Kompas Hotel Aalborg" or "Slotshotellet Aalborg");
            bool isDistinctAndLocalSelected = selectedFilters.Distinct().Count() == 4 && localSelected;
            
            Assert.True(isDistinctAndLocalSelected);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldSelectNoBudgetHotelsIfNoBudgetOptionIsPressed()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "No budget").Click();
            Thread.Sleep(1000); // wait to ensure no budget options are selected
            hotelFilters = _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> selectedFilters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noBudgetSelected = selectedFilters.All(option =>
                option.Text is "No budget" or "Kompas Hotel Aalborg" or "Slotshotellet Aalborg" or
                    "Aalborg Airport Hotel" or "Comwell Hvide Hus Aalborg" or "Helnan Phønix Hotel" or
                    "Hotel Scheelsminde" or "Milling Hotel Aalborg" or "Radisson Blu Limfjord Hotel Aalborg" or 
                    "Scandic Aalborg City" or "Scandic Aalborg Øst");
            bool isDistinctAndNoBudgetSelected = selectedFilters.Distinct().Count() == 11 && noBudgetSelected;
            
            Assert.True(isDistinctAndNoBudgetSelected);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldDeselectAllHotelsIfAllOptionsIsPressedAfterSelectingAllOptions()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "All").Click(); // Select all
            hotelFilters.Single(option => option.Text == "All").Click(); // Deselect all
            Thread.Sleep(1000); // wait to ensure all options are selected then deselected
            hotelFilters = _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> filters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noOptionsSelected = filters.IsNullOrEmpty();
            
            Assert.True(noOptionsSelected);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldDeselectLocalHotelsIfLocalOptionIsPressedAfterSelectingLocalOption()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "Local").Click(); // Select local
            hotelFilters.Single(option => option.Text == "Local").Click(); // Deselect local
            Thread.Sleep(1000); // wait to ensure local option are selected then deselected
            hotelFilters = _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> filters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noOptionsSelected = filters.IsNullOrEmpty();
            
            Assert.True(noOptionsSelected);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldDeselectNoBudgetHotelsIfNoBudgetOptionIsPressedAfterSelectingNoBudgetOption()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "No budget").Click(); // Select no budget
            hotelFilters.Single(option => option.Text == "No budget").Click(); // Deselect no budget
            Thread.Sleep(1000); // wait to ensure no budget option are selected then deselected
            hotelFilters = _wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> filters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noOptionsSelected = filters.IsNullOrEmpty();
            
            Assert.True(noOptionsSelected);
        }

        [Fact]
        public void DashboardPageCalendarForwardArrowShouldMoveCalendarOneMonthForward()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button"))).Click();
            Thread.Sleep(1000); // wait to ensure new month is displayed
            string expectedMonthAndYear = DateTime.Today.AddMonths(1).ToString("MMMM yyyy", new CultureInfo("en-US"));
            string actualMonthAndYear = _wait.Until(webDriver => webDriver.FindElement(By.ClassName("H1MonthAndYear"))).Text;
            
            Assert.Equal(expectedMonthAndYear, actualMonthAndYear);
        }
        
        [Fact]
        public void DashboardPageCalendarBackwardArrowShouldMoveCalendarOneMonthBackIfAlreadyMovedForward()
        {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button"))).Click(); // Move forward
            Thread.Sleep(500); // wait for calendar to switch
            _wait.Until(webDriver => webDriver.FindElement(By.Id("previous-month-button"))).Click(); // Move backward
            Thread.Sleep(1000); // wait to ensure new month is displayed
            string expectedMonthAndYear = DateTime.Today.ToString("MMMM yyyy", new CultureInfo("en-US"));
            string actualMonthAndYear = _wait.Until(webDriver => webDriver.FindElement(By.ClassName("H1MonthAndYear"))).Text;
            
            Assert.Equal(expectedMonthAndYear, actualMonthAndYear);
        }
        
        [Fact]
        public void DashboardPageCalendarBackwardArrowShouldBeDisabledOnCurrentMonth()
        {
            bool backwardArrowEnabled = _wait.Until(webDriver => webDriver.FindElement(By.Id("previous-month-button"))).Enabled;

            Assert.False(backwardArrowEnabled);
        }
        
        [Fact]
        public void DashboardPageCalendarForwardButtonShouldDisableAfterThreePressesFromDefault()
        {
            IWebElement forwardButton = _wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button")));
            for (int i = 1; i <= 3; i++)
            {
                Thread.Sleep(500); // wait between each click
                forwardButton.Click();
            }
            Thread.Sleep(500); // wait for last button to render
            bool forwardArrowEnabled = _wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button"))).Enabled;
            
            Assert.False(forwardArrowEnabled);
        }

        [Fact]
        public void DashboardPageShouldContainEveryWeekday()
        {
            Thread.Sleep(1000); // wait to ensure all weekdays are displayed
            ReadOnlyCollection<IWebElement> listOfWeekdays =
                _wait.Until(webDriver => webDriver.FindElements(By.ClassName("Weekdays")));
            bool allWeekdays = listOfWeekdays.All(day =>
                day.Text is "Monday" or "Tuesday" or "Wednesday" or "Thursday" or "Friday" or "Saturday" or "Sunday");
            bool isDistinctAndContainsAll = listOfWeekdays.Distinct().Count() == 7 && allWeekdays;

            Assert.True(isDistinctAndContainsAll);
        }

        [Fact]
        public void DashboardPageShouldHighlightCurrentDateInCalendar()
        {
            string expectedHighlightDate = DateTime.Today.Day.ToString();
            string actualHighlightedDate = _wait.Until(webDriver => webDriver.FindElement(By.ClassName("bg-warning")))
                .Text.Split('\n')[0];

            Assert.Equal(expectedHighlightDate, actualHighlightedDate);
        }
        
        [Fact]
        public void DashboardPageShouldNotContainPriceThermometerDefaultAtLoad()
        {
            void Action() => _driver.FindElement(By.Id("price-thermometer"));

            Assert.Throws<NoSuchElementException>(Action);
        }
        
        [Fact]
        public void DashboardPageShouldPopOutPriceThermometerWhenADateIsClicked()
        {
            Thread.Sleep(500); // wait for calendar to appear
            _wait.Until(webDriver => webDriver.FindElement(By.ClassName("bg-warning"))).Click(); // Click today
            Thread.Sleep(1000); // wait for price thermometer to animate
            IWebElement priceThermometer = _wait.Until(webDriver => webDriver.FindElement(By.Id("price-thermometer")));

            Assert.NotNull(priceThermometer);
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
            driver.Close();
            driver.Quit();
        }

        public void Dispose()
        {
            Teardown(_driver);
        }
    }
}
