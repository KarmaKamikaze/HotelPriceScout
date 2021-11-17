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

        public async Task<IEnumerable<MarketPriceModel>> DisplaySelectedComparedPrices(List<string> SelectedHotels, string StartDate, string EndDate, int RoomType)
        {
            if (SelectedHotels.Contains("Local"))
            {
                SelectedHotels.Add("Cabinn Aalborg");
                SelectedHotels.Add("Slotshotellet Aalborg");
                SelectedHotels.Add("Kompas Hotel Aalborg");
            }
            if (SelectedHotels.Contains("No budget"))
            {
                SelectedHotels.Add("Kompas Hotel Aalborg");
                SelectedHotels.Add("Slotshotellet Aalborg");
                SelectedHotels.Add("Milling Hotel Aalborg");
                SelectedHotels.Add("Aalborg Airport Hotel");
                SelectedHotels.Add("Helnan Phønix Hotel");
                SelectedHotels.Add("Hotel Schellsminde");
                SelectedHotels.Add("Radisson Blu Limfjord Hotel Aalborg");
                SelectedHotels.Add("Comwell Hvide Hus Aalborg");
                SelectedHotels.Add("Scandic Aalborg Øst");
                SelectedHotels.Add("Scandic Aalborg City");
            }
            var last = SelectedHotels.LastOrDefault();
            foreach (var item in SelectedHotels)
            {
                if (item.Equals(last))
                {
                    All += "'";
                    All += item;
                    All += "'";
                }
                else
                {
                    All += "'";
                    All += item;
                    All += "'";
                    All += " OR ";
                    All += "HotelName = ";
                }
            }
            System.Console.Write(All);
            IEnumerable<MarketPriceModel> SelectedHotelsList = await _db.RetrieveDataFromDb("HotelName, Price, Date", $"RoomType{RoomType}",
                                                                    $"HotelName = {All} AND Date >= '{StartDate}' AND Date <= '{EndDate}'");
            return SelectedHotelsList;
        }

        public async Task<IEnumerable<MarketPriceModel>> DisplayComparedPrices(string StartDate, string EndDate, int RoomType)
        {
            IEnumerable<MarketPriceModel> ComparedPricesList = await _db.RetrieveDataFromDb("Price, Date", "MarketPrices",
                                  $"Date >= '{StartDate}' AND Date <= '{EndDate}' AND RoomType = '{RoomType}'");
            return ComparedPricesList;
        }

        public async Task<IEnumerable<MarketPriceModel>> DisplayKompasPrices(string StartDate, string EndDate, int RoomType)
        {
            IEnumerable<MarketPriceModel> KompasPriceList = await _db.RetrieveDataFromDb("HotelName, Price, Date", $"RoomType{RoomType}",
                                         $"HotelName = 'Kompas Hotel Aalborg' AND Date >= '{StartDate}' AND Date <= '{EndDate}'");
            return KompasPriceList;
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
