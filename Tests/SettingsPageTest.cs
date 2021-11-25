using System;
using AngleSharp.Dom;
using Blazored.Modal;
using Bunit;
using HotelPriceScout.Data.Interface;
using HotelPriceScout.Pages;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using Xunit;

namespace Tests
{
    public class SettingsPageTest : TestContext
    {
        public SettingsPageTest()
        {
            // Inject test services
            JSInterop.SetupVoid("import", _ => true);
            Services.AddSyncfusionBlazor();
            Services.AddBlazoredModal();
            Services.AddSingleton<SettingsManager>();
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
            
            string actualDefaultMargin = cut.Find("#margin-value").Attributes["aria-valuenow"]?.Value;

            Assert.Equal(expectedMarginValue, actualDefaultMargin);
        }
        
        // [Fact]
        // public void SettingsPageShouldContainDefaultNotificationAmountOne()
        // {
        //     IRenderedComponent<Settings> cut = RenderComponent<Settings>();
        //     string expectedNotificationAmountValue = "1";
        //     
        //     string actualNotificationAmountValue = cut.Find("#notification-amount").NodeValue;
        //
        //     Assert.Equal(expectedNotificationAmountValue, actualNotificationAmountValue);
        // }
        
        [Fact]
        public void SettingsPageShouldContainDefaultNotificationTimeTwelveZeroZero()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            string expectedNotificationTimeValue = DateTime.Parse("12:00").ToString("HH:mm");
            
            string actualNotificationTimeValue = cut.Find("#notification-time-1").Attributes["value"]?.Value;

            Assert.Equal(expectedNotificationTimeValue, actualNotificationTimeValue);
        }
        
        // [Fact]
        // public void SettingsPageShouldContainTwoNotificationTimeSelectorsWhenNotificationAmountIsTwo()
        // {
        //     IRenderedComponent<Settings> cut = RenderComponent<Settings>();
        //
        //     cut.Find("#notification-amount").Change(2);
        //     bool thereAreTwoSelectors = cut.Nodes.Contains(cut.Find("#notification-time-1")) && 
        //                                 cut.Nodes.Contains(cut.Find("#notification-time-2"));
        //
        //     Assert.True(thereAreTwoSelectors);
        // }
    }
}