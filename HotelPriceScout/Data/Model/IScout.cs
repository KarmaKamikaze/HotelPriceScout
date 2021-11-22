using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public interface IScout
    {
        int MarginValue { get; }
        IEnumerable<BookingSite> BookingSites { get; }
        IEnumerable<DateTime> NotificationTimes { get; }
        void StartScout();
        void StopScout();
        void RunComparator(string type);
    }
}