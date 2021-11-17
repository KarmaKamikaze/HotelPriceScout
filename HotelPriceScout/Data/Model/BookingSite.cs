using System;
using System.Collections.Generic;
using HotelPriceScout.Data.Function;

namespace HotelPriceScout.Data.Model
{
    public class BookingSite
    {
        private readonly string _type;

        public BookingSite(string name, string type, string url, Dictionary<string, string> hotels)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            HotelsList = CreateHotels(hotels);

            DataScraper = new PseudoScraper(this);
        }

        public string Name { get; }

        public string Type
        {
            get => _type;
            private init
            {
                if (value != "single" && value != "multi")
                {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(value)} must be either \"single\" or \"multi\".");
                }

                _type = value;
            }
        }

        public string Url { get; }

        public IDataScraper DataScraper { get; }

        public IEnumerable<Hotel> HotelsList { get; init; }

        private IEnumerable<Hotel> CreateHotels(Dictionary<string, string> hotelsStrings)
        {
            List<Hotel> hotels = new List<Hotel>();
            foreach (KeyValuePair<string, string> hotel in hotelsStrings)
            {
                hotels.Add(new Hotel(hotel.Key, hotel.Value));
            }

            return hotels;
        }
    }
}