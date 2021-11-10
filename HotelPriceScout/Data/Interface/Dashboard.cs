using Dapper;
using DataAccessLibrary;
using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Interface
{
    public class Dashboard
    {
        public string MonthName { get; private set; } = "";
        public DateTime MonthEnd { get; private set; }
        public int MonthsAway { get; set; } = default;
        public int NumDummyColumn { get; set; } = default;
        public int Year { get; private set; } = default;
        public int Month { get; private set; } = default;
        public int DayClicked { get; set; } = default;
        public DateTime TempDate { get; private set; }
        public DateTime ToDay { get; private set; } = DateTime.Now;
        private SqliteDataAccess _db = new SqliteDataAccess();

        public async Task<IEnumerable<MarketPriceModel>> DisplayComparedPrices(string StartDate, string EndDate)
        {
            TempDate = DateTime.Now.AddMonths(MonthsAway);
            Month = TempDate.Month;
            Year = TempDate.Year;

            IEnumerable<MarketPriceModel> testlist = await _db.RetrieveDataFromDb("Price, Date", "MarketPrices", $"Date >= '{StartDate}' AND Date <= '{EndDate}'");
            
            return testlist;
        }

        public async Task<IEnumerable<MarketPriceModel>> DisplayKompasPrices(string StartDate, string EndDate)
        {
            TempDate = DateTime.Now.AddMonths(MonthsAway);
            Month = TempDate.Month;
            Year = TempDate.Year;

            IEnumerable<MarketPriceModel> testlist = await _db.RetrieveDataFromDb("HotelName, Price, Date", "RoomType1", $"HotelName = 'Kompas Hotel' AND Date >= '{StartDate}' AND Date <= '{EndDate}'");

            return testlist;
        }
       
        public void CreateMonth()
        {
            TempDate = DateTime.Now.AddMonths(MonthsAway);
            Month = TempDate.Month;
            Year = TempDate.Year;

            DateTime monthStart = new DateTime(Year, Month, 1);
            MonthEnd = monthStart.AddMonths(1).AddDays(-1);
            MonthName = monthStart.Month switch
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

            NumDummyColumn = (int)monthStart.DayOfWeek;

            if(NumDummyColumn == 0)
            {
                NumDummyColumn = 7;
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

