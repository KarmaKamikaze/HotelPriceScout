using System;
using System.Collections.Generic;
using System.Linq;


namespace HotelPriceScout.Data.Model
{

    public class Comparator
    {
        private readonly string _type;

        public Comparator()
        {
            Roomtype1HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            Roomtype2HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            Roomtype3HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            AvgMarketPrices = new Dictionary<DateTime, List<decimal>>();
        }

        public bool IsDiscrepancy { get; private set; }

        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype1HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype2HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype3HotelAvgPrices { get; set; }
        private Dictionary<DateTime, List<decimal>> AvgMarketPrices { get; set; }

        public void ComparePrices(IEnumerable<BookingSite> bookingSites, int marginValue)
        {
            DateTime LatestScrapedDate = bookingSites.First().HotelsList.First().RoomTypes.First().Prices.Last().Date;

            for (DateTime date = DateTime.Now.Date; date < LatestScrapedDate; date = date.AddDays(1))
            {
                //These dictionaries describe the values of the key-value-pairs of the RoomtypeXHotelAvgPrices dictionaries.
                Dictionary<string, decimal> dict1 = new();
                Dictionary<string, decimal> dict2 = new();
                Dictionary<string, decimal> dict3 = new();

                //The integer describes the capacity of the roomtypes.
                List<(Dictionary<string, decimal>, int)> dictList = new();
                dictList.Add((dict1, 1));
                dictList.Add((dict2, 2));
                dictList.Add((dict3, 4));

                foreach (BookingSite bookingSite in bookingSites)
                {
                    foreach (Hotel hotel in bookingSite.HotelsList)
                    {
                        foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                        {
                            RoomType roomType = hotel.RoomTypes.Single(r => r.Capacity == capacity);

                            RoomTypePrice roomTypePrice = roomType.Prices.Where(p => p.Date == date).FirstOrDefault();

                            if (roomTypePrice != null)
                            {
                                if (dict.ContainsKey(hotel.Name))
                                {
                                    dict[hotel.Name] = (dict[hotel.Name] + roomTypePrice.Price) / 2;
                                }
                                else
                                {
                                    dict.Add(hotel.Name, roomTypePrice.Price);
                                }
                            }
                        }
                    }
                }

                Roomtype1HotelAvgPrices.Add(date, dict1);
                Roomtype2HotelAvgPrices.Add(date, dict2);
                Roomtype3HotelAvgPrices.Add(date, dict3);

                List<decimal> roomtypeMarketPrices = new();
                foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                {
                    Dictionary<string, decimal> competitorDict = dict;
                    competitorDict.Remove("Kompas Hotel");
                    roomtypeMarketPrices.Add(competitorDict.Values.Average());
                }
                AvgMarketPrices.Add(date, roomtypeMarketPrices);

            }

            DateTime earliestNotifactionDate = AvgMarketPrices.First().Key;
            DateTime latestNotificationDate = earliestNotifactionDate.AddMonths(1);
            for (DateTime date = earliestNotifactionDate; date < latestNotificationDate; date = date.AddDays(1))
            {
                CheckDiscrepancy(date, Roomtype1HotelAvgPrices, marginValue);

                CheckDiscrepancy(date, Roomtype2HotelAvgPrices, marginValue);

                CheckDiscrepancy(date, Roomtype3HotelAvgPrices, marginValue);               
            }





        }

        private void CheckDiscrepancy(DateTime date, Dictionary<DateTime, Dictionary<string, decimal>> hotelAvgPrices, int marginValue)
        {
            if (hotelAvgPrices[date]["Kompas Hotel"] < (1 - (marginValue / 100)) * AvgMarketPrices[date][0] ||
                hotelAvgPrices[date]["Kompas Hotel"] > (1 + (marginValue / 100)) * AvgMarketPrices[date][0])
            {
                IsDiscrepancy = true;
            }
        }


        public void SendNotification()
        {
            throw new NotImplementedException();
        }

    }





}
