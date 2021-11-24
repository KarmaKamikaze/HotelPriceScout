using System.Collections.Generic;
using HotelPriceScout.Data.Model;
using System.Linq;
using System;

namespace HotelPriceScout.Data.Interface
{
        public static class PriceMeterGenerator
        {
            public static List<PriceModel> PriceListGenerator(DateTime todayDate, IEnumerable<PriceModel> monthData, decimal avgPrice)
            {
                List<PriceModel> priceDataList = new();
                priceDataList.AddRange(from PriceModel item in monthData
                                   where (item.Date) == todayDate.Date
                                   select new PriceModel(item.Price, item.HotelName));
                priceDataList.Add(new PriceModel(avgPrice, "Gns. Marked"));
                return priceDataList;
            }
            public static PriceModel MarketFinder(List<PriceModel> list)
            // Finds the market price in the list of prices,
            // This shall not be used if market price comes from somewhere else.
            {
                PriceModel marketPriceItem = list.Find(priceModel => priceModel.HotelName == "Gns. Marked");
                return marketPriceItem;
            }
        }
}
