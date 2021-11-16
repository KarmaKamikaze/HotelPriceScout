using System.Collections.Generic;
using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Function
{
    public interface IDataScraper
    {
        public BookingSite BookingSite { get; }
        void StartScraping(int margin);
    }
}