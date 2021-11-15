using System;
using System.Linq;
using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Function
{
    public class PseudoScraper : IDataScraper
    {
        private const int TYPEONEMIN = 900;
        private const int TYPEONEMAX = 1400;
        private const int TYPETWOMIN = 900;
        private const int TYPETWOMAX = 1400;
        private const int TYPEFOURMIN = 1200;
        private const int TYPEFOURMAX = 1700;
        private const int PROPABILITY = 100 / 5;
        private readonly decimal _hostPriceTypeOne;
        private readonly decimal _hostPriceTypeTwo;
        private readonly decimal _hostPriceTypeFour;
        private readonly Random _random = new();
        
        public PseudoScraper()
        {
            _hostPriceTypeOne = _random.Next(TYPEONEMIN, TYPEONEMAX);
            _hostPriceTypeTwo = _random.Next(TYPETWOMIN, TYPETWOMAX);
            _hostPriceTypeFour = _random.Next(TYPEFOURMIN, TYPEFOURMAX);
        }
        
        public BookingSite BookingSite { get; }

        public void StartScraping(int margin)
        {
            throw new System.NotImplementedException();
        }

        public void SendMissingDataWarning()
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePrices()
        {
            throw new System.NotImplementedException();
        }

        private void AssignRealisticRoomPrices(RoomType room, int margin)
        {
            decimal minPrice;
            decimal maxPrice;
            
            switch (room.Capacity)
            {
                case 1:
                    minPrice = (1 - margin / 100) * _hostPriceTypeOne;
                    maxPrice = (1 + margin / 100) * _hostPriceTypeOne;
                    break;
                case 2:
                    minPrice = (1 - margin / 100) * _hostPriceTypeTwo;
                    maxPrice = (1 + margin / 100) * _hostPriceTypeTwo;
                    break;
                case 4:
                    minPrice = (1 - margin / 100) * _hostPriceTypeFour;
                    maxPrice = (1 + margin / 100) * _hostPriceTypeFour;
                    break;
            }

            foreach (RoomTypePrice price in room.Prices)
            {
                price.Price = RealisticPriceGenerator(margin);
            }
        }

        private decimal RealisticPriceGenerator(int margin)
        {
            _random.Next();


        }

        private bool IsDiscrepancy()
        {
            if (_random.Next(100) < PROPABILITY) return true;
            return false;
        }
    }
}