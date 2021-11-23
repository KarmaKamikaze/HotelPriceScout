using System.Collections.Generic;
using HotelPriceScout.Data.Model;
using System.Linq;
using System;

namespace HotelPriceScout.Pages
{
        public class PriceMeterGenerator
        {
            public static List<PriceModel> PriceListGenerator(DateTime TodayDate, IEnumerable<PriceModel> MonthData, decimal avgPrice) // generates a test list of hotels and their PriceModel.
            {
                List<PriceModel> PriceDataList = new();
            PriceDataList.AddRange(from PriceModel item in MonthData
                                   where (item.Date) == TodayDate.Date
                                   select new PriceModel(item.Price, item.HotelName));
            PriceDataList.Add(new PriceModel(avgPrice, "Gns. Marked"));
                return PriceDataList;
            }
            public static PriceModel MarketFinder(List<PriceModel> list)
            // Finds the market price in the list of prices,
            // This shall not be used if market price comes from somewhere else.
            {
            PriceModel MarketPriceItem = list.Find(list => list.HotelName == "Gns. Marked");
                return MarketPriceItem;
            }
       
    }
}
