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
        public string monthName { get; private set; } = "";
        public DateTime monthEnd { get; private set; }
        public int monthsAway { get; set; } = default;
        public int numDummyColumn { get; set; } = default;
        public int year { get; private set; } = default;
        public int month { get; private set; } = default;
        public int DayClicked { get; set; } = default;
        public DateTime tempDate { get; private set; }
        public DateTime toDay { get; private set; } = DateTime.Now;
        private SqliteDataAccess _db { get; set; } = new SqliteDataAccess();

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

