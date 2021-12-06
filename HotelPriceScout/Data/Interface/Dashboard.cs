using DataAccessLibrary;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using HotelPriceScout.Data.Model;
using System.Runtime.InteropServices;
using System.Linq;

namespace HotelPriceScout.Data.Interface
{
    public class Dashboard : IDashboard
    {
        public List<WarningMessage> WarningMessages { get; } = new List<WarningMessage>();
        public bool BoolExceptionPopup { get; set; } = false;
        public List<PriceModel> PriceList { get; private set; }
        public PriceModel MarketPriceItem { get; private set; }
        public int TempAniDate { get; set; }
        public string MonthName { get; private set; } = "";
        public DateTime MonthEnd { get; private set; }
        public int MonthsAway { get; set; }
        public int NumDummyColumn { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int DayClicked { get; set; }
        public DateTime ToDay { get; } = DateTime.Now;
        public List<string> SelectedHotels { get; private set; } = new List<string>();
        public IEnumerable<string> ListOfHotels { get; set; }
        public IEnumerable<string> LocalList { get; set; }
        public IEnumerable<string> NoBudgetList { get; set; } 
        public DateTime LastDayOfMonth { get; private set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
            DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        private DateTime StartOfMonth { get; set; } =  new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
        private DateTime TempDate { get; set; }
        private const int DataUnavailable = 0;
        private bool CheckForAlternateClick { get; set; } = true;
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
        
        public async Task<IEnumerable<PriceModel>> RetrieveSelectDataFromDb(int roomType, string wantedOutput, [Optional] List<string>  selectedHotels)
        {              
            IEnumerable<PriceModel> dataList = await _db.RetrieveDataFromDb("*", $"RoomType{roomType}",
                                         $" Date >= '{ToDay.ToString("yyyy-MM-dd")}' AND " +
                                         $"Date <= '{LastDayOfMonth.ToString("yyyy-MM-dd")}'");
            List<PriceModel> resultDataList = new List<PriceModel>();
            switch (wantedOutput)
            {
                case "Select Prices" when selectedHotels != null && selectedHotels.Any():
                {
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
                    throw new ArgumentException("Fatal error: Method called without viable WantedOutput parameter");
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

        public void GenerateThermometer(IEnumerable<PriceModel> monthData, IEnumerable<PriceModel> avgMarketPrice)
        {
            DateTime todayDate = new DateTime(Year, Month, DayClicked);
            decimal marketPrice = (avgMarketPrice.Where(date => date.Date == todayDate)).Single().Price;
            PriceList = PriceMeterGenerator.PriceListGenerator(todayDate, monthData, marketPrice);
            MarketPriceItem = PriceMeterGenerator.MarketFinder(PriceList);
            PriceList.Sort();
            
        }
        
        public void UpdateUiMissingDataWarning(BookingSite bookingSite)
        {
            string warnings = "";

            foreach (Hotel hotel in bookingSite.HotelsList)
            {
                foreach (RoomType roomType in hotel.RoomTypes)
                { 
                    foreach(RoomTypePrice price in roomType.Prices)
                    {
                        if(price.Price == 0)
                        {
                            warnings += $"On date: {price.Date.ToString("d", new CultureInfo("da-DK"))} {hotel.Name}, with room type: {roomType.Capacity}|";
                        }
                    }
                }
            }
            WarningMessages.Add(new WarningMessage(warnings, bookingSite.Name));
            BoolExceptionPopup = true;
        }
        
        public void SelectedHotelsChanged(string hotel)
        {
            if (!SelectedHotels.Contains(hotel))
            {
                SelectedHotels.Add(hotel);
                switch (hotel)
                {
                    case "All":
                        foreach (string hotelString in ListOfHotels)
                        {
                            SelectedHotels.Add(hotelString);
                        }
                        break;
                    case "Local":
                        foreach (string hotelString in LocalList)
                        {
                            SelectedHotels.Add(hotelString);
                        }
                        break;
                    case "No budget":
                        foreach (string hotelString in NoBudgetList)
                        {
                            SelectedHotels.Add(hotelString);
                        }
                        break;
                }
            }
            else
            {
                SelectedHotels.Remove(hotel);
                switch (hotel)
                {
                    case "All":
                        SelectedHotels.Clear();
                        break;
                    case "Local":
                        foreach (string hotelString in LocalList)
                        {
                            if (!SelectedHotels.Contains("No budget") || !LocalList.Intersect(NoBudgetList).Contains(hotelString))
                            {
                                SelectedHotels.Remove(hotelString);
                            }
                        }
                        break;
                    case "No budget":
                        foreach (string hotelString in NoBudgetList)
                        {
                            if (!SelectedHotels.Contains("Local") || !NoBudgetList.Intersect(LocalList).Contains(hotelString))
                            {
                                SelectedHotels.Remove(hotelString);
                            }
                        }
                        break;
                }
            }

            int allCount = ListOfHotels.Count(hotelString => SelectedHotels.Contains(hotelString));
            if (allCount == ListOfHotels.Count())
            {
                SelectedHotels.Add("All");
            }
            else
            {
                SelectedHotels.Remove("All");
            }

            int noBudgetCount = NoBudgetList.Count(hotelString => SelectedHotels.Contains(hotelString));
            if (noBudgetCount == NoBudgetList.Count())
            {
                SelectedHotels.Add("No budget");
            }
            else
            {
                SelectedHotels.Remove("No budget");
            }
        
            int localCount = LocalList.Count(hotelString => SelectedHotels.Contains(hotelString));
            if (localCount == LocalList.Count())
            {
                SelectedHotels.Add("Local");
            }
            else
            {
                SelectedHotels.Remove("Local");
            }

            SelectedHotels = SelectedHotels.Distinct().ToList();
        }

        public string ShowCurrentDayAsString()
        {
            return DayClicked.ToString("") + ". " + MonthName;
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

            if (NumDummyColumn == 0)
            {
                NumDummyColumn = 7;
            }
        }
        
        public string ChangeTextColorBasedOnMargin(decimal marketPrice, decimal kompasPrice)
        {
            decimal result = (marketPrice / 100) * SettingsManager.MarginPicked;

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
            decimal result = (marketPrice / 100) * SettingsManager.MarginPicked;
            
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

        public string DetermineAnimation()
        {
            if (DayClicked != 0 && CheckForAlternateClick)
            {
                return "animation1";
            }
            if (DayClicked != 0 && !CheckForAlternateClick && TempAniDate == DayClicked)
            {
                return "animation2";
            }
            return "";
        }

        public string DetermineFocus(int day)
        {
            if (DayClicked != 0 && CheckForAlternateClick && DayClicked == day)
            {
                return "Background-Lightblue";
            }
            if (DayClicked != 0 && !CheckForAlternateClick && TempAniDate == DayClicked)
            {
                return "Background-none";
            }
            return "";
        }
    }
}
