using System;
using System.Collections.Generic;
using DataAccessLibrary;

namespace HotelPriceScout.Data.Model
{
    public interface IScout
    {
        IEnumerable<BookingSite> BookingSites { get; }
        void StartScout();
        void StopScout();
        void RunComparator(string type);
        IEnumerable<PriceModel> RunComparatorForSelectedHotels(DateTime startDate, DateTime endDate,
            IEnumerable<PriceModel> dataList);
    }
}