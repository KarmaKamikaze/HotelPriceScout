using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Tests
{
    public class IndexE2ETests
    {
        [Fact]
        public void IndexRedirectsToSettingsPageIfScoutIsNotRunning()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--ignore-certificate-errors", "headless");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string expectedUrl = "https://localhost:5001/Settings";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
        }
        
        [Fact]
        public void IndexRedirectsToDashboardPageIfScoutIsRunning()
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

            driver.Navigate().GoToUrl("https://localhost:5001/");
            string expectedUrl = "https://localhost:5001/Dashboard";
            bool urlRedirects = wait.Until(webDriver => webDriver.Url == expectedUrl);
            
            Assert.True(urlRedirects);
            
            // Reset shared E2E test state
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            wait.Until(webDriver => webDriver.FindElement(By.Id("stop-button"))).Click();
            wait.Until(webDriver => webDriver.FindElement(By.Id("confirm-stop-button"))).Click();
        }
    }
}