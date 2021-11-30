using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Tests
{
    [Collection("E2E")]
    public class SettingsPageE2ETests
    {
        [Fact]
        public void SettingsPageShouldNotContainStartButtonIfScoutIsStarted()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement startButton = wait.Until(webDriver => webDriver.FindElement(By.Id("start-button")));
            startButton.Click();
            IWebElement stayButton = wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button")));
            stayButton.Click();
            
            void Action() => driver.FindElement(By.Id("start-button"));
            
            Assert.Throws<NoSuchElementException>(Action);
            
            // Reset shared E2E test state
            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }
        
        [Fact]
        public void SettingsPageShouldContainStopButtonIfScoutIsStarted()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement startButton = wait.Until(webDriver => webDriver.FindElement(By.Id("start-button")));
            startButton.Click();
            IWebElement stayButton = wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button")));
            stayButton.Click();

            IWebElement stopButton = driver.FindElement(By.Id("stop-button"));
            
            Assert.NotNull(stopButton);
            
            // Reset shared E2E test state
            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }
        
        [Fact]
        public void SettingsPageShouldContainUpdateButtonIfScoutIsStarted()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement startButton = wait.Until(webDriver => webDriver.FindElement(By.Id("start-button")));
            startButton.Click();
            IWebElement stayButton = wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button")));
            stayButton.Click();

            IWebElement updateButton = driver.FindElement(By.Id("update-button"));
            
            Assert.NotNull(updateButton);
            
            // Reset shared E2E test state
            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }

        [Fact]
        public void SettingsPageShouldContainDashboardRedirectOptionInNavMenu()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            IWebElement homeRedirect = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-home")));
            
            Assert.NotNull(homeRedirect);
        }
        
        [Fact]
        public void SettingsPageShouldNotContainSettingsRedirectOptionInNavMenu()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");

            void Action() => driver.FindElement(By.Id("nav-settings"));
            
            Assert.Throws<NoSuchElementException>(Action);
        }
        
        [Fact]
        public void SettingsPageShouldContainHomeLogoRedirectOptionInNavMenu()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            IWebElement homeRedirect = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-home")));
            
            Assert.NotNull(homeRedirect);
        }
        
        [Fact]
        public void SettingsPageRedirectsToDashboardWhenHomeLogoIsPressedWhenScoutIsRunning()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            IWebElement startButton = wait.Until(webDriver => webDriver.FindElement(By.Id("start-button")));
            startButton.Click();
            IWebElement stayButton = wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button")));
            stayButton.Click();

            IWebElement navLogo = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-logo")));
            navLogo.Click();
            
            string expectedUrl = "https://localhost:5001/Dashboard";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
            
            // Reset shared E2E test state
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }
        
        [Fact]
        public void SettingsPageRedirectsBackToSettingsWhenHomeLogoIsPressedWhenScoutIsNotRunning()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement navLogo = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-logo")));
            navLogo.Click();
            
            string expectedUrl = "https://localhost:5001/Settings";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
        }
        
        [Fact]
        public void SettingsPageRedirectsToDashboardWhenHomeNavIsPressedWhenScoutIsRunning()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            IWebElement startButton = wait.Until(webDriver => webDriver.FindElement(By.Id("start-button")));
            startButton.Click();
            IWebElement stayButton = wait.Until(webDriver => webDriver.FindElement(By.Id("stay-button")));
            stayButton.Click();

            IWebElement navLogo = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-home")));
            navLogo.Click();
            
            string expectedUrl = "https://localhost:5001/Dashboard";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
            
            // Reset shared E2E test state
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }
        
        [Fact]
        public void SettingsPageRedirectsBackToSettingsWhenHomeNavIsPressedWhenScoutIsNotRunning()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement navLogo = wait.Until(webDriver => webDriver.FindElement(By.Id("nav-home")));
            navLogo.Click();
            
            string expectedUrl = "https://localhost:5001/Settings";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
        }
    }
}