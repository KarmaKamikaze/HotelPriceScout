using System;
using System.Threading;
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
            chromeOptions.AddArgument("--ignore-certificate-errors");
            using IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:5001/Settings");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement startButton = wait.Until(element => element.FindElement(By.Id("start-button")));
            startButton.Click();
            IWebElement stayButton = wait.Until(element => element.FindElement(By.Id("stay-button")));
            stayButton.Click();
            
            void Action() => driver.FindElement(By.Id("start-button"));
            
            Assert.Throws<NoSuchElementException>(Action);
            
            wait.Until(element => element.FindElement(By.Id("stop-button"))).Click();
            wait.Until(element => element.FindElement(By.Id("confirm-stop-button"))).Click();
        }
    }
}