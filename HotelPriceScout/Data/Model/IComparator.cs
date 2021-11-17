using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public interface IComparator
    {
        bool IsDiscrepancy { get; }
        void ComparePrices(IEnumerable<BookingSite> bookingSites, int marginValue);
        void SendNotification();
    }
}