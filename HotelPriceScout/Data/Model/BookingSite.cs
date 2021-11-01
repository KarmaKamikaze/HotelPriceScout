using System;

namespace HotelPriceScout.Data.Model
{
    public class BookingSite
    {
        private readonly string _type;

        public BookingSite(string name, string type, string url)
        {
            try
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Type = type ?? throw new ArgumentNullException(nameof(type));
                Url = url ?? throw new ArgumentNullException(nameof(url));
                // TODO: Initialize list of hotels.
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string Name { get; }

        public string Type
        {
            get => _type;
            init
            {
                if (value is "single" or "multi") _type = value;
                else throw new ArgumentOutOfRangeException(
                    $"{nameof(value)} must be either \"single\" or \"multi\".");
            } 
        }
        
        // TODO: List property, containing hotels.

        public string Url { get; }

        private void CreateHotel()
        {
            throw new NotImplementedException();
        }

        public void CreatePriceScraper()
        {
            throw new NotImplementedException();
        }
    }
}