using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DataAccessLibrary;
using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Interface
{
    public interface IDashboard
    {
        List<WarningMessage> WarningMessages { get; }
        bool BoolExceptionPopup { get; set; } //This is used for error occurence in scraping!
        List<PriceModel> PriceList { get; }
        PriceModel MarketPriceItem { get; }
        int TempAniDate { set; }
        string MonthName { get; }
        DateTime MonthEnd { get; }
        int MonthsAway { get; set; }
        int NumDummyColumn { get; }
        int Year { get; }
        int Month { get; }
        int DayClicked { get; set; }
        DateTime ToDay { get; }
        DateTime LastDayOfMonth { get; }
        List<string> SelectedHotels { get; }
        IEnumerable<string> ListOfHotels { get; set; }
        IEnumerable<string> LocalList { set; }
        IEnumerable<string> NoBudgetList { set; }
        decimal GetSingleDayMarketPrice(IEnumerable<PriceModel> multipleMarketPrices, int specificDay);

        Task<IEnumerable<PriceModel>> RetrieveSelectDataFromDb(int roomType, string wantedOutput, [Optional] List<string>  selectedHotels);

        decimal GetSingleDayKompasPrice(IEnumerable<PriceModel> calendarKompasPrices, int specificDay);
        void GenerateThermometer(IEnumerable<PriceModel> monthData, IEnumerable<PriceModel> avgMarketPrice);
        void UpdateUiMissingDataWarning(BookingSite bookingSite);
        string ShowCurrentDayAsString();
        void CreateMonth();
        void SelectedHotelsChanged(string hotel);
        string ChangeTextColorBasedOnMargin(decimal marketPrice, decimal kompasPrice);
        string ArrowDecider(decimal marketPrice, decimal kompasPrice);
        void ShowMoreInfo(int dayClicked);
        void NextMonth();
        void PreviousMonth();
        string DetermineAnimation();
        string DetermineFocus(int day);
    }
}
