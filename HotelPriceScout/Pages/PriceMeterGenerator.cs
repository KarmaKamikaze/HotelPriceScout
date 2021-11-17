using System.Collections.Generic;

namespace HotelPriceScout.Pages
{
        public class PriceMeterGenerator // the generator class
        {
            public static List<Prices> PriceListGenerator() // generates a test list of hotels and their prices.
            {
                List<Prices> PriceDataList = new List<Prices>();
                PriceDataList.Add(new Prices("Hotel Kompas", 599));
                PriceDataList.Add(new Prices("Hotel Radison", 999));
                PriceDataList.Add(new Prices("Hotel Phoenix", 499));
                PriceDataList.Add(new Prices("Hotel Zleep", 799));
                PriceDataList.Add(new Prices("Cabin Centrum", 499));
                PriceDataList.Add(new Prices("Hotel Aalborg", 794));
                PriceDataList.Add(new Prices("Cabin Øst", 499));
                PriceDataList.Add(new Prices("Gns. Marked", 679));
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
