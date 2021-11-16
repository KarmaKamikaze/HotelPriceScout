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
        public string All { get; set; } = "";
        public decimal Average { get; set; } = default;
        public string MonthName { get; private set; } = "";
        public DateTime MonthEnd { get; private set; }
        public int MonthsAway { get; set; } = default;
        public int NumDummyColumn { get; set; } = default;
        public int Year { get; private set; } = default;
        public int Month { get; private set; } = default;
        public int DayClicked { get; set; } = default;
        public DateTime TempDate { get; private set; }
        public DateTime ToDay { get;  set; } = DateTime.Now;
        public DateTime StartOfMonth { get; set; } =  new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
        public DateTime LastDayOfMonth { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1);
        private SqliteDataAccess _db = new SqliteDataAccess();
        public List<decimal> Kage;

        public async Task<decimal> noget(List<string> testS, string StartDate, string EndDate, int RoomType)
        {
            if (testS.Contains("Local"))
            {
                testS.Add("Cabinn Aalborg");
                testS.Add("Slotshotellet Aalborg");
                testS.Add("Kompas Hotel Aalborg");
            }
            if (testS.Contains("No budget"))
            {
                testS.Add("Kompas Hotel Aalborg");
                testS.Add("Slotshotellet Aalborg");
                testS.Add("Milling Hotel Aalborg");
                testS.Add("Aalborg Airport Hotel");
                testS.Add("Helnan Phønix Hotel");
                testS.Add("Hotel Schellsminde");
                testS.Add("Radisson Blu Limfjord Hotel Aalborg");
                testS.Add("Comwell Hvide Hus Aalborg");
                testS.Add("Scandic Aalborg Øst");
                testS.Add("Scandic Aalborg City");
            }

            foreach (string item in testS)
            {
                All += item;
                All += " OR ";
            }
            System.Console.WriteLine(All);
            IEnumerable<MarketPriceModel> testList = await _db.RetrieveDataFromDb("HotelName, Price, Date", $"RoomType{RoomType}",
                                                         $"HotelName = '{All}'AND Date >= '{StartDate}' AND Date <= '{EndDate}'");
            foreach (var item in testList)
            {
                Kage.Add(item.Price);
            }
            Average = Kage.Average();
            return Average;
        }

        public async Task<IEnumerable<MarketPriceModel>> DisplayComparedPrices(string StartDate, string EndDate, int RoomType)
        {
            IEnumerable<MarketPriceModel> testList = await _db.RetrieveDataFromDb("Price, Date", "MarketPrices", $"Date >= '{StartDate}' AND Date <= '{EndDate}' AND RoomType = '{RoomType}'");
            return testList;
        }

        public async Task<IEnumerable<MarketPriceModel>> DisplayKompasPrices(string StartDate, string EndDate, int RoomType)
        {
            IEnumerable<MarketPriceModel> testList = await _db.RetrieveDataFromDb("HotelName, Price, Date", $"RoomType{RoomType}", $"HotelName = 'Kompas Hotel Aalborg' AND Date >= '{StartDate}' AND Date <= '{EndDate}'");
            return testList;
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
            {NumDummyColumn = 7;}
        }

        public void ShowMoreInfo(int dayClicked)
        {
            if (new DateTime(Year, Month, dayClicked, 23, 59, 59) >= DateTime.Now && new DateTime(Year, Month, dayClicked) <= ToDay.AddMonths(3))
            {
                if (dayClicked == DayClicked)
                {
                    DayClicked = 0;
                }
                else
                {
                    DayClicked = dayClicked;
                }
            }
            else
            {
                DayClicked = 0;
            }
        }
        public void NextMonth()
        { 
            StartOfMonth = StartOfMonth.AddMonths(1);
            LastDayOfMonth = StartOfMonth.AddMonths(1).AddDays(-1);
        }
        public void PreviousMonth()
        {
            StartOfMonth = StartOfMonth.AddMonths(-1);
            LastDayOfMonth = StartOfMonth.AddMonths(1).AddDays(-1);
        }
    }
}
