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
        public void SettingsPageShouldNotContainStopButtonIfScoutIsNotStarted()
        {
            IRenderedComponent<Settings> cut = RenderComponent<Settings>();
            
            void Action() => cut.Find("d-flex");

            Assert.Throws<ElementNotFoundException>(Action);
        }
    }
}