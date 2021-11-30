using System.Linq;
using AngleSharp.Dom;
using Blazored.Modal;
using Bunit;
using HotelPriceScout.Data.Interface;
using HotelPriceScout.Pages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    public class SettingsPageTest : TestContext
    {
        public SettingsPageTest()
        {
            // Inject test services
            Services.AddBlazoredModal();
            Services.AddTransient<SettingsManager>();
        }

        [Fact]
        public void SettingsPageShouldNotContainStopButtonIfScoutIsNotStartedWhenFirstOpened()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            
            void Action() => cut.Find("#stop-button");

            Assert.Throws<ElementNotFoundException>(Action);
        }
        
        [Fact]
        public void SettingsPageShouldNotContainUpdateButtonIfScoutIsNotStartedWhenFirstOpened()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            
            void Action() => cut.Find("#update-button");

            Assert.Throws<ElementNotFoundException>(Action);
        }
        
        [Fact]
        public void SettingsPageShouldContainStartButtonIfScoutIsNotStartedWhenFirstOpened()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            
            IElement element = cut.Find("#start-button");

            Assert.NotNull(element);
        }
        
        [Fact]
        public void SettingsPageShouldContainDefaultMarginFifteen()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            string expectedMarginValue = "15";
            
            string actualDefaultMargin = cut.Find("#margin").Attributes["value"]?.Value;

            Assert.Equal(expectedMarginValue, actualDefaultMargin);
        }
        
        [Fact]
        public void SettingsPageShouldContainDefaultNotificationAmountOne()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            string expectedNotificationAmountValue = "1";
            
            string actualNotificationAmountValue = cut.Find("#notification-amount").Attributes["value"]?.Value;
        
            Assert.Equal(expectedNotificationAmountValue, actualNotificationAmountValue);
        }
        
        [Fact]
        public void SettingsPageShouldContainDefaultNotificationTimeTwelveZeroZero()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            string expectedNotificationTimeValue = "12:00:00";
            
            string actualNotificationTimeValue = cut.Find("#notification-time-1").Attributes["value"]?.Value;

            Assert.Equal(expectedNotificationTimeValue, actualNotificationTimeValue);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SettingsPageShouldContainChosenNotificationTimeSelectors(int expectedTimeSelectors)
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            cut.Find("#notification-amount").Change(expectedTimeSelectors);

            IHtmlCollection<IElement> elements = cut.Find("#notification-amount").ParentElement?.ParentElement
                ?.QuerySelectorAll("input.timepicker");
            int actualTimeSelectors = elements.Length;
        
            Assert.Equal(expectedTimeSelectors, actualTimeSelectors);
        }

        [Fact]
        public void SettingsPageShouldContainNotificationTimeOneWhenNotificationAmountIsOne()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            cut.Find("#notification-amount").Change(1);
            
            IElement element = cut.Find("#notification-time-1");

            Assert.NotNull(element);
        }
        
        [Fact]
        public void SettingsPageShouldContainNotificationTimeOneAndTwoWhenNotificationAmountIsTwo()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            cut.Find("#notification-amount").Change(2);

            IRefreshableElementCollection<IElement> elements = cut.FindAll(".timepicker");
            bool twoTimeSelectorsExists =
                elements.All(ele => ele.Id is "notification-time-1" or "notification-time-2") && elements.Count == 2;

            Assert.True(twoTimeSelectorsExists);
        }
        
        [Fact]
        public void SettingsPageShouldContainNotificationTimeOneTwoThreeWhenNotificationAmountIsThree()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            cut.Find("#notification-amount").Change(3);
            
            IRefreshableElementCollection<IElement> elements = cut.FindAll(".timepicker");
            bool threeTimeSelectors =
                elements.All(ele =>
                    ele.Id is "notification-time-1" or "notification-time-2" or "notification-time-3") &&
                elements.Count == 3;

            Assert.True(threeTimeSelectors);
        }
        
        [Fact]
        public void SettingsPageShouldNotContainNotificationTimesWhenNotificationAmountIsZero()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            cut.Find("#notification-amount").Change(0);
            
            IRefreshableElementCollection<IElement> elements = cut.FindAll(".timepicker");
            bool noTimeSelectors = elements.Count == 0;

            Assert.True(noTimeSelectors);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void SettingsPageShouldNotContainNotificationTimeTwoWhenNotificationAmountIsZeroOrOne(int notificationAmount)
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            cut.Find("#notification-amount").Change(notificationAmount);
            
            IRefreshableElementCollection<IElement> elements = cut.FindAll(".timepicker");
            bool noSecondTimeSelector =
                elements.All(ele => ele.Id is "notification-time-1") && elements.Count == notificationAmount;

            Assert.True(noSecondTimeSelector);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SettingsPageShouldNotContainNotificationTimeThreeWhenNotificationAmountIsZeroOrOneOrTwo(int notificationAmount)
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            cut.Find("#notification-amount").Change(notificationAmount);
            
            IRefreshableElementCollection<IElement> elements = cut.FindAll(".timepicker");
            bool noThirdTimeSelector =
                elements.All(ele => ele.Id is "notification-time-1" or "notification-time-2") && elements.Count == notificationAmount;

            Assert.True(noThirdTimeSelector);
        }
    }
}