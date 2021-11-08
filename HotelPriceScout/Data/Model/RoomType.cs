using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{

    public class RoomType
    {
        private int _capacity;

        public RoomType(int capacity)
        {
            Capacity = capacity;
            Prices = Create3monthsprices();
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value != 1 && value != 2 && value != 4)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} has to be 1, 2 or 4");
                }
                _capacity = value;
            }
        }

        public List<RoomTypePrice> Prices { get; init; }

        private List<RoomTypePrice> Create3monthsprices()
        {
            List<RoomTypePrice> prices = new();
            int i = 0;
            for (DateTime currentdate = DateTime.Now.Date; currentdate.AddDays(i) < currentdate.AddMonths(3); i++)
            {
                prices.Add(new RoomTypePrice(currentdate.AddDays(i)));
            }
            return prices;
        }
    }
}