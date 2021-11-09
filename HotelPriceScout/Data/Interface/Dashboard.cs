using Dapper;
using DataAccessLibrary;
using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace HotelPriceScout.Data.Interface
{
    public class Dashboard
    {
        public string monthName = "";
        public DateTime monthEnd;
        public int monthsAway = default;
        public int numDummyColumn = default;
        public int year = default;
        public int month = default;
        public int DayClicked = default;
        public int temp = default;
        public DateTime tempDate;
        public DateTime toDay = DateTime.Now;
        private SqliteDataAccess _db = new SqliteDataAccess();

        public async Task<IEnumerable<MarketPriceModel>> DisplayComparedPrices(string StartDate, string EndDate)
        {
            tempDate = DateTime.Now.AddMonths(monthsAway);
            month = tempDate.Month;
            year = tempDate.Year;

            IEnumerable<MarketPriceModel> testlist = await _db.RetrieveDataFromDb("Price, Date", "MarketPrices", $"Date >= '{StartDate}' AND Date <= '{EndDate}'");
            
            return testlist;
        }

        public void CreateMonth()
        {
            tempDate = DateTime.Now.AddMonths(monthsAway);
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

