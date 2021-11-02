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
            Create3monthsprices();
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value is not 1 or 2 or 4)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} has to be 1, 2 or 4");
                }
                _capacity = value;
            }
        }

        public List<RoomTypePrice> Prices { get; private set; }

        public void Create3monthsprices()
        {
            Prices = new List<RoomTypePrice>();
            int i = 0;
            for (DateTime currentdate = DateTime.Now; currentdate.AddDays(i) < currentdate.AddMonths(3); i++)
            {
                Prices.Add(new RoomTypePrice(currentdate.AddDays(i)));
            }
        }

    }
}