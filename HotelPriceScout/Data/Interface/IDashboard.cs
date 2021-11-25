using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Interface
{
    public interface IDashboard
    {
        List<PriceModel> PriceList { get; }
        PriceModel MarketPriceItem { get; }
        int TempAniDate { get; set; }
        bool CheckForAlternateClick { get; }
        string MonthName { get; }
        DateTime MonthEnd { get; }
        int MonthsAway { get; set; }
        int NumDummyColumn { get; }
        int Year { get; }
        int Month { get; }
        int DayClicked { get; set; }
        DateTime ToDay { get; }
        DateTime LastDayOfMonth { get; }
        decimal GetSingleDayMarketPrice(IEnumerable<PriceModel> multipleMarketPrices, int specificDay);

        Task<IEnumerable<PriceModel>> RetrieveSelectDataFromDb(DateTime startDate, DateTime endDate, 
            int roomType, string wantedOutput, [Optional] List<string>  selectedHotels);

        decimal GetSingleDayKompasPrice(IEnumerable<PriceModel> calendarKompasPrices, int specificDay);
        void GenerateThermometer(int day, IEnumerable<PriceModel> monthData, List<PriceModel> avgMarketPrice);
        void UpdateUiMissingDataWarning(BookingSite bookingSite);
        string ShowCurrentDayAsString();
        void CreateMonth();
        string ChangeTextColorBasedOnMargin(decimal marketPrice, decimal kompasPrice);
        string ArrowDecider(decimal marketPrice, decimal kompasPrice);
        decimal CurrentMargin(decimal marketPrice);
        void ShowMoreInfo(int dayClicked);
        void NextMonth();
        void PreviousMonth();
        string DetermineAnimation(int dayClicked, bool checkForAlternateClick, int tempAniDate);
        string DetermineFocus(int dayClicked, bool checkForAlternateClick, int tempAniDate, int day);
    }
}