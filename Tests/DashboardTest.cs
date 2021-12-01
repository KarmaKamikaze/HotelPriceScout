using System;
using Xunit;
using HotelPriceScout.Data.Interface;
using HotelPriceScout.Data.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Dapper;

namespace Tests
{
    public class DashboardTest
    {
        private ITestOutputHelper _testOutputHelper;
        public void XUnitTestClass(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }


        public static readonly object[][] ShowMoreInfoData =
        {
            new object[] {false, 22, null},
            new object[] {false, DateTime.Now.Day, DateTime.Now.Day},
            new object[] {true, DateTime.Now.Day, (DateTime.Now.Day + 1)}
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

        [Theory, MemberData(nameof(ShowMoreInfoData))]
        public void Test_If_ShowMoreInfo_Returns_Correct_Value(bool expected, int currentDay, int previousDateClicked)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            dashboard.CreateMonth();
            dashboard.DayClicked = previousDateClicked;
            dashboard.ShowMoreInfo(currentDay);
            //Assert
            Assert.Equal(expected, dashboard.CheckForAlternateClick);
        }

        //[Fact]
        //public void Test_If_UpdateUiMissingDataWarning_Returns_Warning()
        //{
        //    //Arrange
        //    Dashboard dashboard = new Dashboard();
        //    Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
        //    {
        //        { "hotel1", "tag1" }
        //    };
        //    RoomType roomType = new RoomType(1);
        //    //Act
        //    BookingSite bookingSite = new BookingSite("hotel1", "single", "https://www.url.com", hotelStrings);

        //    bookingSite.HotelsList.First().RoomTypes.First().Prices.First().Price = 0;

        //    dashboard.UpdateUiMissingDataWarning(bookingSite);

        //    List<WarningMessage> test = new List<WarningMessage>();
        //    test.Add(new WarningMessage($"On date: {DateTime.Now.Date} hotel: {bookingSite.Name}, with roomtype: {bookingSite.Type}|", bookingSite.Name));

        //    //Assert.Equal(test, dashboard.WarningMessage);

        //    Assert.Equal(test)
        //}

        [Fact]
        public void Test_If_UpdateUiMissingData_Returns_Correct_Values_In_WarningMessage()
        {
            Dashboard dashboard = new Dashboard();
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "hotel1", "tag1" }
            };
            BookingSite bookingSite = new BookingSite("hotel1", "single", "https://www.url.com", hotelStrings);

            bookingSite.HotelsList.First().RoomTypes.First().Prices.First().Price = 0;
            WarningMessage warningMessage = new WarningMessage("", "");

            dashboard.UpdateUiMissingDataWarning(bookingSite);

            string dash = string.Join(", ", warningMessage.ListofWarnings);

            Assert.Equal("deez", dash);

            //Assert.All(dashboard.WarningMessage,
              //  ListofWarnings => Assert.Contains($"{DateTime.Now.Date}", ListofWarnings.ToString())
                //); ;
        }
    }
}
