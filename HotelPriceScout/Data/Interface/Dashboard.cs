using DataAccessLibrary;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using HotelPriceScout.Data.Model;
using System.Runtime.InteropServices;
using System.Linq;
using HotelPriceScout.Pages;

namespace HotelPriceScout.Data.Interface
{
    public class Dashboard
    {
        public List<PriceModel> priceList { get; private set; }
        public PriceModel MarketPriceItem { get; private set; }
        private const int DATAUNAVAILABLE = 0;
        public int TempAniDate { get; set; }
        public bool CheckForAlternateClick { get; set; } = true;
        public string AllSelectedHotels { get; set; } = "";
        public string MonthName { get; private set; } = "";
        public DateTime MonthEnd { get; private set; }
        public int MonthsAway { get; set; }
        public int NumDummyColumn { get; set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int DayClicked { get; set; }
        public DateTime TempDate { get; private set; }
        public DateTime ToDay { get;  set; } = DateTime.Now;
        public DateTime StartOfMonth { get; set; } =  new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
        public DateTime LastDayOfMonth { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1);
        private readonly SqliteDataAccess _db = new();

        public IEnumerable<PriceModel> SelectedMonthMarketPrices(DateTime startDate, DateTime endDate, IEnumerable<PriceModel> dataList)
        {
            List<decimal> TempList = new();
            List<PriceModel> ListOfSingleDatePrices = new();
            for(DateTime tempDate = startDate; tempDate <= endDate; tempDate = tempDate.AddDays(1))
            {
                TempList.AddRange(from item in dataList
                                  where item.Date == tempDate
                                  select item.Price);
                PriceModel SingleDayMarketPrice = new PriceModel(TempList.Average(), tempDate);
                ListOfSingleDatePrices.Add(SingleDayMarketPrice);
            }
            dataList = ListOfSingleDatePrices;
            return dataList;
        }
        public decimal GetSingleDayMarketPrice(IEnumerable<PriceModel> multipleMarketPrices, int specificDay)
        {   
            //The time is set to 23:59:59 to ensure that no matter the time of loading the data, the current day will be correct
            if (new DateTime(Year, Month, specificDay, 23, 59, 59) >= ToDay &&
                new DateTime(Year, Month, specificDay) <= ToDay.AddMonths(3))
            {
                    return multipleMarketPrices.Single(mp => mp.Date == new DateTime(Year, Month, specificDay).Date).Price;
            }
            return DATAUNAVAILABLE;
        }
        public async Task<IEnumerable<PriceModel>> RetrieveSelectDataFromDb(DateTime startDate, DateTime endDate, int roomType, string wantedOutput, [Optional] List<string>  selectedHotels)
        {              
            IEnumerable<PriceModel> dataList = await _db.RetrieveDataFromDb("*", $"RoomType{roomType}",
                                         $" Date >= '{startDate.ToString("yyyy-MM-dd")}' AND Date <= '{endDate.ToString("yyyy-MM-dd")}'");
            List<PriceModel> resultDataList = new();
            if (wantedOutput == "Select Prices")
            {
                if (selectedHotels != null) 
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
                else 
                {
                    return dataList; //if no hotels are selected all data is returned
                }
            }
            else if (wantedOutput == "Kompas Prices")
            {
                resultDataList.AddRange(from item in dataList
                                      where item.HotelName == "Kompas Hotel Aalborg"// picks all Kompas Hotel prices
                                      select item);
                
                return resultDataList;
            }
            throw new Exception("Fatal error: Method Called without WantedOutput parameter");
        }
        public decimal GetSingleDayKompasPrice(IEnumerable<PriceModel> calendarKompasPrices, int specificDay)
        {
            //The time is set to 23:59:59 to ensure that no matter the time of loading the data, the current day will be correct
            if (new DateTime(Year, Month, specificDay, 23, 59, 59) >= ToDay &&
                new DateTime(Year, Month, specificDay) <= ToDay.AddMonths(3))
            {
                return calendarKompasPrices.Single(mp => mp.Date == new DateTime(Year, Month, specificDay) && mp.HotelName == "Kompas Hotel Aalborg").Price;
            }
            return DATAUNAVAILABLE;
        }
       
        public void GenerateThermometer(int day, int monthaway, IEnumerable<PriceModel> monthData)
        {
            DateTime todayDate = new DateTime(Year, Month, day);
            todayDate.AddMonths(monthaway);
            priceList = PriceMeterGenerator.PriceListGenerator(todayDate, monthData);
            MarketPriceItem = PriceMeterGenerator.MarketFinder(priceList);
            priceList.Sort();
        }
        public void UpdateUiMissingDataWarning(BookingSite bookingSite)
        {
            throw new NotImplementedException();
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
        public string ChangeTextColorBasedOnMargin(decimal marketprice, decimal kompasPrice)
        {
            decimal result = (kompasPrice / 100) * SettingsManager.marginPickedPass;

            if (marketprice > (kompasPrice + result))
            {
                return "low";
            }
            else if (marketprice < (kompasPrice - result))
            {
                return "high";
            }
            else
            {
                return "";
            }
        }
        public string ArrowDecider(decimal marketPrice, decimal kompasPrice)
        {

            decimal result = (kompasPrice / 100) * SettingsManager.marginPickedPass;

            if (marketPrice > (kompasPrice + result))
            {
                return "oi oi-caret-top";
            }
            else if (marketPrice < (kompasPrice - result))
            {
                return "oi oi-caret-bottom";
            }
            else
            {
                return "oi oi-minus";
            }
        }
        public void ShowMoreInfo(int dayClicked)
        {
            if (new DateTime(Year, Month, dayClicked, 23, 59, 59) >= DateTime.Now && new DateTime(Year, Month, dayClicked) <= ToDay.AddMonths(3))
            {
                if (dayClicked == DayClicked)
                {
                    CheckForAlternateClick = false;
                }
                else
                {
                    CheckForAlternateClick = true;
                    DayClicked = dayClicked;
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
    }
}
