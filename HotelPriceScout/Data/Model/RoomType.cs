using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public class RoomType
    {
        private readonly int _capacity;

        public RoomType(int capacity)
        {
            Capacity = capacity;
            Prices = CreateThreeMonthsPrices();
        }

        public int Capacity
        {
            get => _capacity;
            private init
            {
                if (value != 1 && value != 2 && value != 4)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} has to be 1, 2 or 4");
                }
                _capacity = value;
            }
        }

        public List<RoomTypePrice> Prices { get; }

        private List<RoomTypePrice> CreateThreeMonthsPrices()
        {
            List<RoomTypePrice> prices = new();
            int i = 0;
            for (DateTime currentDate = DateTime.Now.Date; currentDate.AddDays(i) <= currentDate.AddMonths(3); i++)
            {
                prices.Add(new RoomTypePrice(currentDate.AddDays(i)));
            }
            return prices;
        }
    }
}