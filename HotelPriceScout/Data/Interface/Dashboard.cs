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

        public IEnumerable<MarketPriceModel> SelectedMonthMarketPrices(DateTime StartDate, DateTime EndDate, IEnumerable<MarketPriceModel> DataList)
        {
            List<int> TempList = new();
            List<MarketPriceModel> ListOfSingelDatePrices = new();
            //DateTime tempDate = StartDate;
            for(DateTime tempDate = StartDate; tempDate <= EndDate;)
            {
                TempList.AddRange(from item in DataList
                                  where item.Date == tempDate
                                  select item.Price);
                MarketPriceModel SingelDayMarketPrice = new MarketPriceModel((int)TempList.Average(), tempDate);
                ListOfSingelDatePrices.Add(SingelDayMarketPrice);
                tempDate = tempDate.AddDays(1);
            }
            DataList = ListOfSingelDatePrices;
            return DataList;
        }
        public int SingleDayMarketPrice(IEnumerable<MarketPriceModel> MultipleMarketPrices, int SpeceficDay)
        {
            //The time is set to 23:59:59 to ensure that no matter the time of loading the data, the current day will be correct
            if (new DateTime(Year, Month, SpeceficDay, 23, 59, 59) >= ToDay &&
                new DateTime(Year, Month, SpeceficDay) <= ToDay.AddMonths(3))
            {
                foreach (var item in from item in MultipleMarketPrices
                                     where (item.Date).Date == new DateTime(Year, Month, SpeceficDay).Date
                                     select item/*throw new Exception($"Market price for date:{new DateTime(Year, Month, SpeceficDay)}, was not in list of marketprices ");*/)
                {
                    return item.Price;
                }
            }
            return 0;
        }
        public async Task<IEnumerable<MarketPriceModel>> RetrieveSelectDataFromDb(DateTime StartDate, DateTime EndDate, int RoomType, string WantedOutput, [Optional] List<string>  SelectedHotels)
        {              
            IEnumerable<MarketPriceModel> DataList = await _db.RetrieveDataFromDb("*", $"RoomType{RoomType}",
                                         $" Date >= '{StartDate.ToString("yyyy-MM-dd")}' AND Date <= '{EndDate.ToString("yyyy-MM-dd")}'");
            List<MarketPriceModel> tempDataList = new();
            if (WantedOutput == "Select Prices")
            {
                if (SelectedHotels != null) 
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
                    tempDataList.AddRange(from item in DataList
                                          where SelectedHotels.Contains(item.HotelName)
                                          select item);
                    DataList = tempDataList; // as the temp list is not an ienumerable it is put in DataList which is, and returned
                    return DataList;
                }
                else 
                {
                     return DataList; //if no hotels are selected alle data is returned
                }
            }
            else if (WantedOutput == "Kompas Prices")
            {
                tempDataList.AddRange(from item in DataList
                                      where item.HotelName == "Kompas Hotel Aalborg"// picks all HotelKompas prices
                                      select item);
                // as the temp list is not an ienumerable it is put in DataList which is, and returned
                return tempDataList;
            }
            throw new Exception("Fatal error: Method Called without WantedOutput parameter");
        }
        public int SingleDayKompasPrice(IEnumerable<MarketPriceModel> CalendarKompasPrices, int SpeceficDay)
        {
            //The time is set to 23:59:59 to ensure that no matter the time of loading the data, the current day will be correct
            if (new DateTime(Year, Month, SpeceficDay, 23, 59, 59) >= ToDay &&
                new DateTime(Year, Month, SpeceficDay) <= ToDay.AddMonths(3))
            {
                IEnumerable<MarketPriceModel> SingleKompasPrice = CalendarKompasPrices
                    .Where(MarketPriceModel => MarketPriceModel.Date == new DateTime(Year, Month, SpeceficDay)
                    && MarketPriceModel.HotelName == "Kompas Hotel Aalborg");
                return SingleKompasPrice.Single().Price;
            }
            return 0;
        }
        public List<Prices> priceList;
        public Prices MarketPriceItem;
        public void GenerateThermometer(int Day,int Monthaway, IEnumerable<MarketPriceModel> MonthData, int MarketPrice)
        {  // This needs to get its data from the database, as a list of Price objects
            DateTime TodayDate = new DateTime(Year, Month, Day);
            TodayDate.AddMonths(Monthaway);
            priceList = PriceMeterGenerator.PriceListGenerator(TodayDate, MonthData);
            MarketPriceItem = (PriceMeterGenerator.MarketFinder(priceList));
            priceList.Sort();
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

            NumDummyColumn = (int)monthStart.DayOfWeek;

            if(NumDummyColumn == 0)
            {NumDummyColumn = 7;}
        }
        public string ChangeTextColorBasedOnMargin(int Marketprice, int KompasPrice)
        {
            int result = (KompasPrice / 100) * SettingsManager.marginPickedPass;

            if (Marketprice > (KompasPrice + result))
            {
                return "low";
            }
            else if (Marketprice < (KompasPrice - result))
            {
                return "high";
            }
            else
            {
                return "";
            }
        }
        public string ArrowDecider(int MarketPrice, int KompasPrice)
        {

            int result = (KompasPrice / 100) * SettingsManager.marginPickedPass;

            if (MarketPrice > (KompasPrice + result))
            {
                return "oi oi-caret-top";
            }
            else if (MarketPrice < (KompasPrice - result))
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
