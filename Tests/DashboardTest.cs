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
            new object[] {true, DateTime.Now.Day, DateTime.Now.Day},
            new object[] {true, DateTime.Now.Day, (DateTime.Now.Day + 1)}
        };


        [Fact]
        public void Test_If_CreateMonth_Creates_Correct_Month_Based_On_The_Current_Month()
        {
            //Arrange and Act
            Dashboard dashboard = new Dashboard();
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
            //Arrange and Act
            Dashboard dashboard = new Dashboard();
            string dash = dashboard.ChangeTextColorBasedOnMargin(marketprice, kompasPrice);
            //Assert
            Assert.Equal(expected, dash);
        }

        [Theory]
        [InlineData("oi oi-caret-top", 100, 1)]
        [InlineData("oi oi-caret-bottom", 1, 100)]
        [InlineData("oi oi-minus", 0, 0)]
        public void Test_If_ArrowDecider_Returns_Correct_Value(string expected, int marketprice, int kompasPrice)
        {
            //Arrange and Act
            Dashboard dashboard = new Dashboard();
            string dash = dashboard.ArrowDecider(marketprice, kompasPrice);
            //Assert
            Assert.Equal(expected, dash);
        }

        [Theory, MemberData(nameof(ShowMoreInfoData))]
        public void Test_If_ShowMoreInfo_Returns_Correct_Value(bool expected, int dayClicked, int DayClicked)
        {
            //Arrange and Act
            Dashboard dashboard = new Dashboard();
            dashboard.CreateMonth();
            dashboard.ShowMoreInfo(dayClicked);
            dashboard.DayClicked = DayClicked;
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
            //Arrange and Act
            Dashboard dashboard = new Dashboard();
            string dash = dashboard.DetermineAnimation(DayClicked, CheckForAlternateClick, TempAniDate);
            //Assert
            Assert.Equal(expected, dash);
        }
    }
}
