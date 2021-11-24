using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using HotelPriceScout.Data.Model;
using HotelPriceScout.Pages;
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
        public void Test_If_ChangeTextColorBasedOnMargin_Returns_Correct_Expected_Value(string expected, int marketprice, int kompasPrice)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            string actual = dashboard.ChangeTextColorBasedOnMargin(marketprice, kompasPrice);
            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("oi oi-caret-top", 100, 1)]
        [InlineData("oi oi-caret-bottom", 1, 100)]
        [InlineData("oi oi-minus", 0, 0)]
        public void Test_If_ArrowDecider_Returns_Correct_Value(string expected, int marketprice, int kompasPrice)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            string actual = dashboard.ArrowDecider(marketprice, kompasPrice);
            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(ShowMoreInfoData))]
        public void Test_If_ShowMoreInfo_Returns_Correct_Value(bool expected, int currentday, int DayClicked)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            dashboard.CreateMonth();
            dashboard.DayClicked = DayClicked;
            dashboard.ShowMoreInfo(currentday);
            //Assert
            Assert.Equal(expected, dashboard.CheckForAlternateClick);
        }

        [Theory]
        [InlineData("animation1", 5, true, 0)]
        [InlineData("animation2", 5, false, 5)]
        [InlineData("", 5, false, 4)]
        [InlineData("", 0, false, 0)]
        public void Test_If_DetermineAnimation_Returns_Correct_Value(string expected, int DayClicked, bool CheckForAlternateClick, int TempAniDate)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            string actual = dashboard.DetermineAnimation(DayClicked, CheckForAlternateClick, TempAniDate);
            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
