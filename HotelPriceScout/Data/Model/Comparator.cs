using System;
using System.Collections.Generic;
using System.Linq;


namespace HotelPriceScout.Data.Model
{

    public class Comparator
    {
        private readonly string _type;

        public Comparator(string type, IEnumerable<BookingSite> bookingSites, int marginValue)
        {
            Type = type;
            BookingSites = bookingSites;
            MarginValue = marginValue;
        }

        private bool IsDiscrepancy { get; set; }

        public string Type
        {
            get => _type;
            init
            {
                if (value is "dashboard" or "email") _type = value;
                else throw new ArgumentOutOfRangeException(
                    $"{nameof(value)} must be either \"dashboard\" or \"email\".");
            }
        }

        public IEnumerable<BookingSite> BookingSites { get; init; }
        public int MarginValue { get; init; }

        public void ComparePrices()
        {
            Dictionary<DateTime, Dictionary<string, decimal>> roomtype1HotelAvgPrices = new();
            Dictionary<DateTime, Dictionary<string, decimal>> roomtype2HotelAvgPrices = new();
            Dictionary<DateTime, Dictionary<string, decimal>> roomtype3HotelAvgPrices = new();
            Dictionary<DateTime, List<decimal>> marketPrices = new();


            for (DateTime date = DateTime.Now; date < date.AddMonths(3); date = date.AddDays(1))
            {
                Dictionary<string, decimal> dict1 = new();
                Dictionary<string, decimal> dict2 = new();
                Dictionary<string, decimal> dict3 = new();

                //The integer describes the capacity of the roomtypes.
                List<(Dictionary<string, decimal>, int)> dictList = new();
                dictList.Add((dict1, 1));
                dictList.Add((dict2, 2));
                dictList.Add((dict3, 4));

                foreach (BookingSite bookingSite in BookingSites)
                {
                    foreach (Hotel hotel in bookingSite.HotelsList)
                    {
                        foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                        {
                            RoomType roomType = hotel.RoomTypes.Where(r => r.Capacity == capacity).First();

                            RoomTypePrice roomTypePrice = roomType.Prices.Find(p => p.Date == date);

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

                roomtype1HotelAvgPrices.Add(date, dict1);
                roomtype2HotelAvgPrices.Add(date, dict2);
                roomtype3HotelAvgPrices.Add(date, dict3);
                List<decimal> list = new();
                foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                {
                    decimal accumulatedPrice = 0;
                    foreach(KeyValuePair<string, decimal> keyValuePair in dict)
                    {
                        accumulatedPrice += keyValuePair.Value;
                    }
                    
                    list.Add(accumulatedPrice / dict.Count());
                }
                marketPrices.Add(date, list);
            }
        }

        public void SendNotification()
        {
            throw new NotImplementedException();
        }

    }





}
