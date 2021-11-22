using System.Collections.Generic;
using HotelPriceScout.Data.Model;
using System.Linq;
using System;

namespace HotelPriceScout.Pages
{
        public class PriceMeterGenerator
        {
            public static List<MarketPriceModel> PriceListGenerator(DateTime TodayDate, IEnumerable<MarketPriceModel> MonthData) // generates a test list of hotels and their prices.
            {
                List<MarketPriceModel> PriceDataList = new();
                PriceDataList.AddRange(from MarketPriceModel item in MonthData
                                       where (item.Date) == TodayDate.Date
                                       select new MarketPriceModel( item.Price, item.HotelName));
                decimal MarketPrice = PriceDataList.Average(x => x.Price);
                PriceDataList.Add(new MarketPriceModel(MarketPrice, "Gns. Marked"));
                PriceDataList.Sort();
                return PriceDataList;
            }
            public static MarketPriceModel MarketFinder(List<MarketPriceModel> list)
            // Finds the market price in the list of prices,
            // This shall not be used if market price comes from somewhere else.
            {
            MarketPriceModel MarketPriceItem = list.Find(list => list.HotelName == "Gns. Marked");
                return MarketPriceItem;
            }
       
    }
}
