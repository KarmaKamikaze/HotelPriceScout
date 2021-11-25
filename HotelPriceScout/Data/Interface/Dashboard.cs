using DataAccessLibrary;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using HotelPriceScout.Data.Model;
using System.Runtime.InteropServices;
using System.Linq;

namespace HotelPriceScout.Data.Interface
{
    public class Dashboard : IDashboard
    {
        public List<WarningMessagde> WarningMessagde { get; set; } = new List<WarningMessagde>();
        public bool BoolExceptionPopup { get; set; } = false;
        public List<PriceModel> PriceList { get; private set; }
        public PriceModel MarketPriceItem { get; private set; }
        private const int DataUnavailable = 0;
        public int TempAniDate { get; set; }
        public bool CheckForAlternateClick { get; private set; } = true;
        public string MonthName { get; private set; } = "";
        public DateTime MonthEnd { get; private set; }
        public int MonthsAway { get; set; }
        public int NumDummyColumn { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int DayClicked { get; set; }
        private DateTime TempDate { get; set; }
        public DateTime ToDay { get; } = DateTime.Now;
        private DateTime StartOfMonth { get; set; } =  new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
        public DateTime LastDayOfMonth { get; private set; } = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1);
        private readonly ISqliteDataAccess _db = new SqliteDataAccess();
        
        public decimal GetSingleDayMarketPrice(IEnumerable<PriceModel> multipleMarketPrices, int specificDay)
        {   
            //The time is set to 23:59:59 to ensure that no matter the time of loading the data, the current day will be correct
            if (new DateTime(Year, Month, specificDay, 23, 59, 59) >= ToDay &&
                new DateTime(Year, Month, specificDay) <= ToDay.AddMonths(3))
            {
                    return multipleMarketPrices.Single(mp => mp.Date == new DateTime(Year, Month, specificDay).Date).Price;
            }
            return DataUnavailable;
        }
        public async Task<IEnumerable<PriceModel>> RetrieveSelectDataFromDb(DateTime startDate, DateTime endDate, 
            int roomType, string wantedOutput, [Optional] List<string>  selectedHotels)
        {              
            IEnumerable<PriceModel> dataList = await _db.RetrieveDataFromDb("*", $"RoomType{roomType}",
                                         $" Date >= '{startDate.ToString("yyyy-MM-dd")}' AND " +
                                         $"Date <= '{endDate.ToString("yyyy-MM-dd")}'");
            List<PriceModel> resultDataList = new();
            switch (wantedOutput)
            {
                case "Select Prices" when selectedHotels != null:
                {
                    if (selectedHotels.Contains("Local"))
                    {
                        selectedHotels.Add("Cabinn Aalborg");
                        selectedHotels.Add("Slotshotellet Aalborg");
                        selectedHotels.Add("Kompas Hotel Aalborg");
                    }
                    if (selectedHotels.Contains("No budget"))
                    {
                        selectedHotels.Add("Kompas Hotel Aalborg");
                        selectedHotels.Add("Slotshotellet Aalborg");
                        selectedHotels.Add("Milling Hotel Aalborg");
                        selectedHotels.Add("Aalborg Airport Hotel");
                        selectedHotels.Add("Helnan Phønix Hotel");
                        selectedHotels.Add("Hotel Schellsminde");
                        selectedHotels.Add("Radisson Blu Limfjord Hotel Aalborg");
                        selectedHotels.Add("Comwell Hvide Hus Aalborg");
                        selectedHotels.Add("Scandic Aalborg Øst");
                        selectedHotels.Add("Scandic Aalborg City");
                    }
                    resultDataList.AddRange(from item in dataList
                        where selectedHotels.Contains(item.HotelName)
                        select item);
                    return resultDataList.Distinct();
                }
                case "Select Prices":
                    return dataList; //if no hotels are selected all data is returned
                case "Kompas Prices":
                    resultDataList.AddRange(from item in dataList
                        where item.HotelName == "Kompas Hotel Aalborg"// picks all Kompas Hotel prices
                        select item);
                    return resultDataList;
                default:
                    throw new Exception("Fatal error: Method Called without WantedOutput parameter");
            }
        }
        
        public decimal GetSingleDayKompasPrice(IEnumerable<PriceModel> calendarKompasPrices, int specificDay)
        {
            //The time is set to 23:59:59 to ensure that no matter the time of loading the data, the current day will be correct
            if (new DateTime(Year, Month, specificDay, 23, 59, 59) >= ToDay &&
                new DateTime(Year, Month, specificDay) <= ToDay.AddMonths(3))
            {
                return calendarKompasPrices.Single(mp => mp.Date == new DateTime(Year, Month, specificDay) && 
                                                         mp.HotelName == "Kompas Hotel Aalborg").Price;
            }
            return DataUnavailable;
        }

        public void GenerateThermometer(int day, IEnumerable<PriceModel> monthData, List<PriceModel> avgMarketPrice)
        {
            DateTime todayDate = new(Year, Month, day);
            decimal marketPrice = (avgMarketPrice.Where(date => date.Date == todayDate)).Single().Price;
            PriceList = PriceMeterGenerator.PriceListGenerator(todayDate, monthData, marketPrice);
            MarketPriceItem = PriceMeterGenerator.MarketFinder(PriceList);
            PriceList.Sort();
            
        }
        public void UpdateUiMissingDataWarning(BookingSite bookingSite)
        {
            string warnings = "";
            string bookingSitename = bookingSite.Name; 

            foreach (var hotel in bookingSite.HotelsList)
            {
                string hotelName = hotel.Name;
                foreach (var roomtype in hotel.RoomTypes)
                { 
                    string type = roomtype.Capacity.ToString();
                    foreach(var price in roomtype.Prices)
                    {
                        if(price.Price == 0)
                        {
                            warnings += $"On date: {price.Date} hotel: {hotelName}, with roomtype: {roomtype}|";
                        }
                    }
                }
            }
            
            //BoolExceptionPopup = !BoolExceptionPopup; THis bool should be flicked when we are done generating Warnings.

            //WarningMessagde.Add(new WarningMessagde("Date: 21/02/2021 hotel: Phønix, with roomtype: 1|Date: 21/02/2021 hotel:Cabin, with roomtype: 4|Date: 21/02/2021 hotel:Sarmilan, with roomtype: 1", "Expedia.com"));
            //WarningMessagde.Add(new WarningMessagde("Date: 21/02/2021 hotel: Phønix, with roomtype: 2|Date: 21/02/2021 hotel:Cabin, with roomtype: 2|Date: 21/02/2021 hotel:Sarmilan, with roomtype: 4", "Hotels.com"));
            //WarningMessagde.Add(new WarningMessagde("Date: 21/02/2021 hotel: Phønix, with roomtype: 4|Date: 21/02/2021 hotel:Cabin, with roomtype: 1|Date: 21/02/2021hotel:Sarmilan, with roomtype: 2", "Trivago.dk"));


        }
        public string ShowCurrentDayAsString()
        {
            return DayClicked.ToString("") + ". " + MonthName + " " + Year.ToString("");
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
                9 => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => ""
            };

            NumDummyColumn = Convert.ToInt32(monthStart.DayOfWeek);

            if(NumDummyColumn == 0)
            {NumDummyColumn = 7;}
        }
        public string ChangeTextColorBasedOnMargin(decimal marketPrice, decimal kompasPrice)
        {
            decimal result = (marketPrice / 100) * SettingsManager.marginPicked;
            


            if (kompasPrice > (marketPrice + result))
            {
                return "high";
            }

            if (kompasPrice < (marketPrice - result))
            {
                return "low";
            }

            return "";
        }
        public string ArrowDecider(decimal marketPrice, decimal kompasPrice)
        {
            decimal result = (marketPrice / 100) * SettingsManager.marginPicked;
            if (kompasPrice > (marketPrice + result))
            {
                return "oi oi-caret-bottom";
            }

            if (kompasPrice < (marketPrice - result))
            {
                
                return "oi oi-caret-top";
            }

            return "oi oi-minus";
        }
        public decimal CurrentMargin(decimal marketPrice)
        {
            decimal result = (marketPrice / 100) * SettingsManager.marginPicked;

            return result;
        }
        public void ShowMoreInfo(int dayClicked)
        {
            if (new DateTime(Year, Month, dayClicked, 23, 59, 59) >= DateTime.Now && 
                new DateTime(Year, Month, dayClicked) <= ToDay.AddMonths(3))
            {
                if (dayClicked == DayClicked)
                {
                    CheckForAlternateClick = !CheckForAlternateClick;
                }
                else
                {
                    DayClicked = dayClicked;
                    CheckForAlternateClick = true;
                }
            }
            else
            {
                CheckForAlternateClick = false;
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

        public string DetermineAnimation(int dayClicked, bool checkForAlternateClick, int tempAniDate)
        {
            if (dayClicked != 0 && checkForAlternateClick)
            {
                return "animation1";
            }
            
            if (dayClicked != 0 && !checkForAlternateClick && tempAniDate == dayClicked)
            {
                return "animation2";
            }
            return "";
        }
    }
}
