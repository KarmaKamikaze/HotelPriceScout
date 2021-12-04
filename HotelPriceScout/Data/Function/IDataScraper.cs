using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Function
{
    public interface IDataScraper
    {
        public event MissingDataWarning SendMissingDataWarning;
        BookingSite BookingSite { get; }
        void StartScraping(decimal margin);
    }
}