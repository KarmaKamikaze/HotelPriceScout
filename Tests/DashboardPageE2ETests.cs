using System;
using System.Collections.ObjectModel;
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
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(webDriver => webDriver.FindElement(By.Id("start-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button"))).Click();
            driver.Navigate().GoToUrl("https://localhost:5001/Dashboard");

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
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(webDriver => webDriver.FindElement(By.Id("start-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button"))).Click();
            driver.Navigate().GoToUrl("https://localhost:5001/Dashboard");

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
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(webDriver => webDriver.FindElement(By.Id("start-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button"))).Click();
            driver.Navigate().GoToUrl("https://localhost:5001/Dashboard");

            string expectedMonthAndYear = DateTime.Today.ToString("MMMM yyyy");
            string actualMonthAndYear = wait.Until(webDriver => webDriver.FindElement(By.ClassName("H1MonthAndYear"))).Text;
            
            Assert.Equal(expectedMonthAndYear, actualMonthAndYear);
            Teardown(driver);
        }
        
        [Theory]
        [InlineData("Monday", 0)]
        [InlineData("Tuesday", 1)]
        [InlineData("Wednesday", 2)]
        [InlineData("Thursday", 3)]
        [InlineData("Friday", 4)]
        [InlineData("Saturday", 5)]
        [InlineData("Sunday", 6)]
        public void DashboardPageShouldContainEveryWeekday(string expectedWeekday, int daysSinceMonday)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(webDriver => webDriver.FindElement(By.Id("start-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button"))).Click();
            driver.Navigate().GoToUrl("https://localhost:5001/Dashboard");
            
            ReadOnlyCollection<IWebElement> listOfWeekdays =
                wait.Until(webDriver => webDriver.FindElements(By.ClassName("Weekdays")));
            string actualWeekday = listOfWeekdays[daysSinceMonday].Text;
            
            Assert.Equal(expectedWeekday, actualWeekday);
            Teardown(driver);
        }

        private static void Teardown(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }
    }
}