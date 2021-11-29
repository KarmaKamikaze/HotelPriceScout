using System;
using Xunit;
using HotelPriceScout.Data.Interface;

namespace Tests
{
    public class DashboardTest
    {
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
    }
}
