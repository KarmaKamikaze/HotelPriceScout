using System;
using Dapper;
using DataAccessLibrary;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace HotelPriceScout.Data.Interface
{
    public class Dashboard : SqliteDataAccess
    {
        public string monthName = "";
        public DateTime monthEnd;
        public int monthsAway = default;
        public int numDummyColumn = default;
        public int year = default;
        public int month = default;
        public int DayClicked = default;

        public void CreateMonth()
        {
            var tempDate = DateTime.Now.AddMonths(monthsAway);
            month = tempDate.Month;
            year = tempDate.Year;

            DateTime monthStart = new DateTime(year, month, 1);
            monthEnd = monthStart.AddMonths(1).AddDays(-1);
            monthName = monthStart.Month switch
            {
                1 => "January",
                2 => "February",
                3 => "March",
                4 => "April",
                5 => "May",
                6 => "June",
                7 => "July",
                8 => "August",
                9 => "Septemeber",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => ""
            };

            numDummyColumn = (int)monthStart.DayOfWeek;

            if(numDummyColumn == 0)
            {
                numDummyColumn = 7;
            }
        }

        public async void DisplayComaredPrices(int Day)
        {
            IEnumerable<marketprice> testliste = await Retrivedatafromdb("Price, Date", "MarketPrices", "Date < '2021-12-01'");

            IEnumerable<marketprice> Daprice = testliste.Where(date => date.Date == new DateTime(year, month, Day));
            foreach (marketprice price in Daprice)
            {
                System.Console.WriteLine(price.Price);
            }
        }

        public void ShowMoreInfo(int DayClicked)
        {
            if (DayClicked == this.DayClicked)
            {
                this.DayClicked = 0;
            }
            else
            {
                this.DayClicked = DayClicked;
            }
        }
    }
}

