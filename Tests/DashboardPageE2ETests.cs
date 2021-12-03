using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class DashboardPageE2ETests
    {

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
        public void DashboardPageFilterDropdownShouldSelectAllHotelsIfAllOptionsIsPressed()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "All").Click();
            Thread.Sleep(1000); // wait to ensure all options are selected
            hotelFilters = wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            bool allOptionsSelected = hotelFilters.All(option => option.GetAttribute("class").Contains("selected"));
            
            Assert.True(allOptionsSelected);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldSelectLocalHotelsIfLocalOptionIsPressed()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "Local").Click();
            Thread.Sleep(1000); // wait to ensure local options are selected
            hotelFilters = wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> selectedFilters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool localSelected = selectedFilters.All(option =>
                option.Text is "Local" or "Cabinn Aalborg" or "Kompas Hotel Aalborg" or "Slotshotellet Aalborg");
            bool isDistinctAndLocalSelected = selectedFilters.Distinct().Count() == 4 && localSelected;
            
            Assert.True(isDistinctAndLocalSelected);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldSelectNoBudgetHotelsIfNoBudgetOptionIsPressed()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "No budget").Click();
            Thread.Sleep(1000); // wait to ensure no budget options are selected
            hotelFilters = wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> selectedFilters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noBudgetSelected = selectedFilters.All(option =>
                option.Text is "No budget" or "Kompas Hotel Aalborg" or "Slotshotellet Aalborg" or
                    "Aalborg Airport Hotel" or "Comwell Hvide Hus Aalborg" or "Helnan Phønix Hotel" or
                    "Hotel Scheelsminde" or "Milling Hotel Aalborg" or "Radisson Blu Limfjord Hotel Aalborg" or 
                    "Scandic Aalborg City" or "Scandic Aalborg Øst");
            bool isDistinctAndLocalSelected = selectedFilters.Distinct().Count() == 11 && noBudgetSelected;
            
            Assert.True(isDistinctAndLocalSelected);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldDeselectAllHotelsIfAllOptionsIsPressedAfterSelectingAllOptions()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "All").Click(); // Select all
            hotelFilters.Single(option => option.Text == "All").Click(); // Deselect all
            Thread.Sleep(1000); // wait to ensure all options are selected then deselected
            hotelFilters = wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> filters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noOptionsSelected = filters.IsNullOrEmpty();
            
            Assert.True(noOptionsSelected);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldDeselectLocalHotelsIfLocalOptionIsPressedAfterSelectingLocalOption()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "Local").Click(); // Select local
            hotelFilters.Single(option => option.Text == "Local").Click(); // Deselect local
            Thread.Sleep(1000); // wait to ensure local option are selected then deselected
            hotelFilters = wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> filters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noOptionsSelected = filters.IsNullOrEmpty();
            
            Assert.True(noOptionsSelected);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageFilterDropdownShouldDeselectNoBudgetHotelsIfNoBudgetOptionIsPressedAfterSelectingNoBudgetOption()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("selected-hotels-button"))).Click();
            Thread.Sleep(1000); // wait to ensure all options are displayed
            ReadOnlyCollection<IWebElement> hotelFilters =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            hotelFilters.Single(option => option.Text == "No budget").Click(); // Select no budget
            hotelFilters.Single(option => option.Text == "No budget").Click(); // Deselect no budget
            Thread.Sleep(1000); // wait to ensure no budget option are selected then deselected
            hotelFilters = wait.Until(webDriver => webDriver.FindElements(By.ClassName("HotelOptions")));
            IEnumerable<IWebElement> filters =
                hotelFilters.Where(option => option.GetAttribute("class").Contains("selected")).ToList();
            bool noOptionsSelected = filters.IsNullOrEmpty();
            
            Assert.True(noOptionsSelected);
            Teardown(driver);
        }

        [Fact]
        public void DashboardPageCalendarForwardArrowShouldMoveCalendarOneMonthForward()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button"))).Click();
            Thread.Sleep(1000); // wait to ensure new month is displayed
            string expectedMonthAndYear = DateTime.Today.AddMonths(1).ToString("MMMM yyyy");
            string actualMonthAndYear = wait.Until(webDriver => webDriver.FindElement(By.ClassName("H1MonthAndYear"))).Text;
            
            Assert.Equal(expectedMonthAndYear, actualMonthAndYear);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageCalendarBackwardArrowShouldMoveCalendarOneMonthBackIfAlreadyMovedForward()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button"))).Click(); // Move forward
            Thread.Sleep(500); // wait for calendar to switch
            wait.Until(webDriver => webDriver.FindElement(By.Id("previous-month-button"))).Click(); // Move backward
            Thread.Sleep(1000); // wait to ensure new month is displayed
            string expectedMonthAndYear = DateTime.Today.ToString("MMMM yyyy");
            string actualMonthAndYear = wait.Until(webDriver => webDriver.FindElement(By.ClassName("H1MonthAndYear"))).Text;
            
            Assert.Equal(expectedMonthAndYear, actualMonthAndYear);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageCalendarBackwardArrowShouldBeDisabledOnCurrentMonth()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            bool backwardArrowEnabled = wait.Until(webDriver => webDriver.FindElement(By.Id("previous-month-button"))).Enabled;

            Assert.False(backwardArrowEnabled);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageCalendarForwardButtonShouldDisableAfterThreePressesFromDefault()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            IWebElement forwardButton = wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button")));
            for (int i = 1; i <= 3; i++)
            {
                Thread.Sleep(500); // wait between each click
                forwardButton.Click();
            }
            Thread.Sleep(500); // wait for last button to render
            bool forwardArrowEnabled = wait.Until(webDriver => webDriver.FindElement(By.Id("next-month-button"))).Enabled;
            
            Assert.False(forwardArrowEnabled);
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
        
        [Fact]
        public void DashboardPageShouldNotContainPriceThermometerDefaultAtLoad()
        {
            using IWebDriver driver = Setup();

            void Action() => driver.FindElement(By.Id("price-thermometer"));

            Assert.Throws<NoSuchElementException>(Action);
            Teardown(driver);
        }
        
        [Fact]
        public void DashboardPageShouldPopOutPriceThermometerWhenADateIsClicked()
        {
            using IWebDriver driver = Setup();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            Thread.Sleep(500); // wait for calendar to appear
            wait.Until(webDriver => webDriver.FindElement(By.ClassName("bg-warning"))).Click(); // Click today
            Thread.Sleep(1000); // wait for price thermometer to animate
            IWebElement priceThermometer = wait.Until(webDriver => webDriver.FindElement(By.Id("price-thermometer")));

            Assert.NotNull(priceThermometer);
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
            driver.Close();
            driver.Quit();
        }
    }
}