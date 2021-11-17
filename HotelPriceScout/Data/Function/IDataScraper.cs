using System;
using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Function
{
    public interface IDataScraper
    {
        BookingSite BookingSite { get; }
        void StartScraping(decimal margin);
    }
}