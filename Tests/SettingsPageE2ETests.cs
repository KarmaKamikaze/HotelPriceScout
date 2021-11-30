using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
            Thread.Sleep(1000);
            
            IWebElement startButton = driver.FindElement(By.Id("start-button"));
            startButton.Click();
            Thread.Sleep(1000);
            IWebElement stayButton = driver.FindElement(By.Id("stay-button"));
            stayButton.Click();
            
            void Action() => driver.FindElement(By.Id("start-button"));
            
            Assert.Throws<NoSuchElementException>(Action);
            
            driver.FindElement(By.Id("stop-button")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.Id("confirm-stop-button")).Click();
        }
    }
}