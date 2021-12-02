using System;
using Xunit;
using HotelPriceScout.Data.Interface;
using HotelPriceScout.Data.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Tests
{
    public class DashboardTest
    {
        public static IEnumerable<object[]> Getvalues =>
            new List<object[]>
        {
                new object[] {DateTime.Now.Date.ToString()},
                new object[] {"hotel"},
                new object[] {"1"}
        };




        [Fact]
        public void Test_If_CreateMonth_Creates_Correct_Month_Based_On_The_Current_Month()
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            dashboard.CreateMonth();
            //Assert
            Assert.Equal(DateTime.Now.Month, dashboard.Month);
        }

        [Theory]
        [InlineData("low", 100, 1)]
        [InlineData("high", 1, 100)]
        [InlineData("", 0, 0)]
        public void Test_If_ChangeTextColorBasedOnMargin_Returns_Correct_Expected_Value(string expected, int marketPrice, int kompasPrice)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            string actual = dashboard.ChangeTextColorBasedOnMargin(marketPrice, kompasPrice);
            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("oi oi-caret-top", 100, 1)]
        [InlineData("oi oi-caret-bottom", 1, 100)]
        [InlineData("oi oi-minus", 0, 0)]
        public void Test_If_ArrowDecider_Returns_Correct_Value(string expected, int marketPrice, int kompasPrice)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            string actual = dashboard.ArrowDecider(marketPrice, kompasPrice);
            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(Getvalues))]
        public void Test_If_UpdateUiMissingData_Returns_Correct_Values_In_WarningMessage(string expectedSubstring)
        {
            Dashboard dashboard = new Dashboard();
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                {"hotel", "tag" }
            };
            BookingSite bookingSite = new BookingSite("bookingSite", "single", "https://www.url.com", hotelStrings);

            bookingSite.DataScraper.StartScraping(10);

            bookingSite.HotelsList.First().RoomTypes.First().Prices.First().Price = 0;

            dashboard.UpdateUiMissingDataWarning(bookingSite);

            string actual = dashboard.WarningMessages.First().ConcatenatedWarningString;

            Assert.Contains(expectedSubstring, actual);
        }
    }
}
