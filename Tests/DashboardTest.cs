﻿using System;
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
        public static readonly object[][] GetSingleDayMarketPriceInfo =
        {
            //As the Year and Month attributes of
            new object[] {DateTime.Now.AddDays(-1).Day},
            new object[] {DateTime.Now.AddDays(100).Day}
        };
        [Theory, MemberData(nameof(GetSingleDayMarketPriceInfo))]
        public void Test_If_GetSingleDayMarketPrice_Returns_Zero(int specificDay)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.CreateMonth();

            IEnumerable<PriceModel> multipleMarketPrices = Enumerable.Empty<PriceModel>();

            var data = dashboard.GetSingleDayMarketPrice(multipleMarketPrices, specificDay);

            Assert.Equal(0, data);
        }
        [Theory, MemberData(nameof(GetSingleDayMarketPriceInfo))]
        public void Test_If_GetSingleDayMarketPrice_Returns_Correct_Price(int specificDay)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.CreateMonth();

            IEnumerable<PriceModel> multipleMarketPrices = Enumerable.Empty<PriceModel>();

            var data = dashboard.GetSingleDayMarketPrice(multipleMarketPrices, specificDay);

        }



        public static readonly object[][] GetSingleDayMarketPriceInfoData =
        {
            new object[] {DateTime.Now.Day, DateTime.Now.AddDays(1).Day}
        };
        [Theory, MemberData(nameof(GetSingleDayMarketPriceInfoData))]
        public async Task Test_If_GetSingleDayMarketPrice_Returns_Price(DateTime startday, DateTime endday)
        {
            //Arrange and Act
            Dashboard dashboard = new Dashboard();
            dashboard.CreateMonth();
            var data = await dashboard.RetrieveSelectDataFromDb(startday, endday, 1, "Kompas Prices");

            //Assert
            Assert.Equal(0, dashboard.GetSingleDayKompasPrice(data, 5));
        }

        public void Test_If_RetrieveSelectDataFromDB_Returns_Correct_Results()
        {

        }

        public void Test_If_RetrieveSelectDataFromDB_Throws_Error()
        {

        }

        public void Test_If_GetSingleDayKompasPrice_Returns_Zero()
        {

        }

        public void Test_If_GetSingleDayKompasPrice_Returns_Price()
        {

        }

        public void Test_If_GenerateThermometer_Returns_Sorted_List()
        {

        }

        public void Test_If_GenerateThermometer_Returns_Right_Day_Clicked()
        {

        }

        public void Test_If_UpdateUiMissingDataWarning_Throws_Error()
        {
            //Hasn't been created yet
        }

        [Fact]
        public void Test_If_CreateMonth_Creates_Correct_Month()
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

        public void Test_If_ChangeTextColorBasedOnMargin_Returns_Correct_Value_Low(string expected, int marketprice, int kompasPrice)
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

        public static readonly object[][] ShowMoreInfoData =
        {
            new object[] {false, 22, null},
            new object[] {true, DateTime.Now.Day, DateTime.Now.Day},
            new object[] {true, DateTime.Now.Day, (DateTime.Now.Day + 1)}
        };

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
