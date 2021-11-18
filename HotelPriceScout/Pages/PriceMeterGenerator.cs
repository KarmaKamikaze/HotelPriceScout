using System.Collections.Generic;
using HotelPriceScout.Data.Model;
using System.Linq;

namespace HotelPriceScout.Pages
{
        public class PriceMeterGenerator // the generator class
        {
            public static List<Prices> PriceListGenerator(int Day, IEnumerable<MarketPriceModel> MonthData) // generates a test list of hotels and their prices.
            {
                List<Prices> PriceDataList = new();
                PriceDataList.AddRange(from MarketPriceModel item in MonthData
                                   where (item.Date).Day == Day
                                   select new Prices(item.HotelName, item.Price));
                int MarketPrice = (int)PriceDataList.Average(x => x.Price);
                PriceDataList.Add(new Prices("Gns. Marked", MarketPrice));
                PriceDataList.Sort();
                return PriceDataList;
            }
            public static Prices MarketFinder(List<Prices> list)
            // finds the market price in the list of prices,
            // This shall not be used if market price comes from somewhere else.
            {
                Prices MarketPriceItem = list.Find(list => list.Name == "Gns. Marked");
                return MarketPriceItem;
            }
    }
}
