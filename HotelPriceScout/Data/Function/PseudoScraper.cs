using System;
using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Function
{
    public class PseudoScraper : IDataScraper
    {
        private const int TYPE_ONE_MIN = 900;
        private const int TYPE_ONE_MAX = 1400;
        private const int TYPE_TWO_MIN = 900;
        private const int TYPE_TWO_MAX = 1400;
        private const int TYPE_FOUR_MIN = 1200;
        private const int TYPE_FOUR_MAX = 1700;
        private const int DISCREPANCY_PROBABILITY = 100 / 5;
        private const int PRICE_CHANGE_PROBABILITY = 100 / 15;
        private const int ABOVE_OR_BELOW_MARGIN_PROBABILITY = 100 / 50;
        private const int VARIANCE = 200;
        private readonly decimal _hostPriceTypeOne;
        private readonly decimal _hostPriceTypeTwo;
        private readonly decimal _hostPriceTypeFour;
        private readonly Random _random = new();
        private bool _firstTimeUpdate;
        
        public PseudoScraper(BookingSite bookingSite)
        {
            BookingSite = bookingSite ?? throw new NullReferenceException();
            _hostPriceTypeOne = _random.Next(TYPE_ONE_MIN, TYPE_ONE_MAX);
            _hostPriceTypeTwo = _random.Next(TYPE_TWO_MIN, TYPE_TWO_MAX);
            _hostPriceTypeFour = _random.Next(TYPE_FOUR_MIN, TYPE_FOUR_MAX);
        }
        
        public BookingSite BookingSite { get; }

        public void StartScraping(int margin)
        {
            // Sets the initial room type prices for all 90 days.
            _firstTimeUpdate = true;
            UpdatePrices(margin);
            _firstTimeUpdate = false;
            
            // TODO: Do the rest of the infinite scraping loop using timed events so we do not block
        }

        private void SendMissingDataWarning() // May ned to be event/delegate pair
        {
            throw new System.NotImplementedException();
        }

        private void UpdatePrices(int margin)
        {
            foreach (Hotel hotel in BookingSite.HotelsList)
            {
                foreach (RoomType roomType in hotel.RoomTypes)
                {
                    AssignRoomPrices(roomType, margin);
                }
            }
        }

        private void AssignRoomPrices(RoomType room, int margin)
        {
            switch (room.Capacity)
            {
                case 1:
                    SetPrice(room, margin, _hostPriceTypeOne);
                    break;
                case 2:
                    SetPrice(room, margin, _hostPriceTypeTwo);
                    break;
                case 4:
                    SetPrice(room, margin, _hostPriceTypeFour);
                    break;
            }
        }

        private void SetPrice(RoomType room, int margin, decimal hostPriceType)
        {
            decimal maxPrice = (1 + margin / 100) * hostPriceType;
            decimal minPrice = (1 - margin / 100) * hostPriceType;
            
            foreach (RoomTypePrice price in room.Prices)
            {
                if (_firstTimeUpdate || CheckOutcome(PRICE_CHANGE_PROBABILITY))
                {
                    if (CheckOutcome(DISCREPANCY_PROBABILITY))
                    {
                        price.Price = CheckOutcome(ABOVE_OR_BELOW_MARGIN_PROBABILITY)
                            ? _random.Next((int) maxPrice, (int) (maxPrice + VARIANCE))
                            : _random.Next((int) (minPrice - VARIANCE), (int) minPrice);
                    }
                    else
                    {
                        price.Price = _random.Next((int) minPrice, (int) maxPrice);
                    }
                }
            }
        }

        private bool CheckOutcome(int weight)
        {
            if (_random.Next(100) < weight) return true;
            return false;
        }
    }
}