using System;
using System.Collections.Generic;
using DataAccessLibrary;

namespace HotelPriceScout.Data.Model
{
    public interface IComparator
    {
        bool IsDiscrepancy { get; }
        void ComparePrices(IEnumerable<BookingSite> bookingSites, int marginValue);
        void SendNotification();
        IEnumerable<PriceModel> OneMonthSelectedHotelsMarketPrices(DateTime startDate, DateTime endDate,
            IEnumerable<PriceModel> dataList);
    }
}