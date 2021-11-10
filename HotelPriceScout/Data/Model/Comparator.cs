using DataAccessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HotelPriceScout.Data.Model
{

    public class Comparator
    {
        private SqliteDataAccess _db = new SqliteDataAccess();

        public Comparator()
        {
            Roomtype1HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            Roomtype2HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            Roomtype4HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            AvgMarketPrices = new Dictionary<DateTime, List<decimal>>();
        }

        public bool IsDiscrepancy { get; private set; }

        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype1HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype2HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype4HotelAvgPrices { get; set; }
        private Dictionary<DateTime, List<decimal>> AvgMarketPrices { get; set; }

        public async void ComparePrices(IEnumerable<BookingSite> bookingSites, int marginValue)
        {
            DateTime LatestScrapedDate = bookingSites.First().HotelsList.First().RoomTypes.First().Prices.Last().Date;

            for (DateTime date = DateTime.Now.Date; date <= LatestScrapedDate; date = date.AddDays(1))
            {
                //These dictionaries describe the values of the key-value-pairs of the RoomtypeXHotelAvgPrices dictionaries.
                Dictionary<string, decimal> dict1 = new();
                Dictionary<string, decimal> dict2 = new();
                Dictionary<string, decimal> dict4 = new();

                //The integer describes the capacity of the roomtypes.
                List<(Dictionary<string, decimal>, int)> dictList = new();
                dictList.Add((dict1, 1));
                dictList.Add((dict2, 2));
                dictList.Add((dict4, 4));

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
                Roomtype4HotelAvgPrices.Add(date, dict4);

                List<decimal> roomtypeMarketPrices = new();
                foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                {
                    roomtypeMarketPrices.Add(dict.Values.Average());
                }
                AvgMarketPrices.Add(date, roomtypeMarketPrices);

            }

            DateTime earliestNotifactionDate = AvgMarketPrices.First().Key;
            DateTime latestNotificationDate = earliestNotifactionDate.AddMonths(1);
            for (DateTime date = earliestNotifactionDate; date < latestNotificationDate; date = date.AddDays(1))
            {
                CheckDiscrepancy(date, Roomtype1HotelAvgPrices, marginValue);
                CheckDiscrepancy(date, Roomtype2HotelAvgPrices, marginValue);
                CheckDiscrepancy(date, Roomtype4HotelAvgPrices, marginValue);               
            }

            StoreAvgHotelPrices(Roomtype1HotelAvgPrices, "RoomType1");
            StoreAvgHotelPrices(Roomtype2HotelAvgPrices, "RoomType2");
            StoreAvgHotelPrices(Roomtype4HotelAvgPrices, "RoomType4");

            string valueDB = $"INSERT INTO MarketPrices (Date,Price,RoomType) VALUES ";
            foreach (KeyValuePair<DateTime, List<decimal>> datePricesPair in AvgMarketPrices)
            {
                int roomTypeIdentifier = 1;
                foreach (decimal avgPrice in datePricesPair.Value)
                {
                    valueDB += $"('{datePricesPair.Key.ToString("yyyy-MM-dd")}','{avgPrice}','{roomTypeIdentifier}'),";
                    roomTypeIdentifier++;
                    if (roomTypeIdentifier == 3) roomTypeIdentifier++;                   
                }
            }

            valueDB = valueDB.TrimEnd(',');
            valueDB += ";";

            await _db.SaveToDB<dynamic>($"DROP TABLE IF EXISTS MarketPrices;", new { });
            await _db.SaveToDB<dynamic>($"CREATE TABLE [MarketPrices] ([Date] date NOT NULL, [Price] decimal NOT NULL, [RoomType] int NOT NULL);", new { });
            await _db.SaveToDB<dynamic>(valueDB, new { });

        }

        public async void StoreAvgHotelPrices(Dictionary<DateTime, Dictionary<string, decimal>> roomtypeHotelAvgPrices, string tableName)
        {
            string valueDB = $"INSERT INTO {tableName} (Date,HotelName,Price) VALUES ";
            foreach (KeyValuePair<DateTime, Dictionary<string, decimal>> dateHotelsPair in roomtypeHotelAvgPrices)
            {
                foreach (KeyValuePair<string, decimal> hotelPricePair in dateHotelsPair.Value)
                {
                    valueDB += $"('{dateHotelsPair.Key.ToString("yyyy-MM-dd")}','{hotelPricePair.Key}','{hotelPricePair.Value}'),";
                }
            }
            valueDB = valueDB.TrimEnd(',');
            valueDB += ";";

            await _db.SaveToDB<dynamic>($"DROP TABLE IF EXISTS {tableName};", new { });
            await _db.SaveToDB<dynamic>($"CREATE TABLE [{tableName}] ([HotelName] text NOT NULL, [Price] decimal NOT NULL, [Date] date NOT NULL);", new { });
            await _db.SaveToDB<dynamic>(valueDB, new { });
        }


        private void CheckDiscrepancy(DateTime date, Dictionary<DateTime, Dictionary<string, decimal>> hotelAvgPrices, int marginValue)
        {
            if (hotelAvgPrices[date]["Kompas Hotel Aalborg"] < (1 - (marginValue / 100)) * AvgMarketPrices[date][0] ||
                hotelAvgPrices[date]["Kompas Hotel Aalborg"] > (1 + (marginValue / 100)) * AvgMarketPrices[date][0])
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
