using System;

namespace HotelPriceScout.Data.Model
{
    public class MarketPriceModel
    {
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public string HotelName { get; set;  }
        public bool MarkedForDiscrepancy { get; set; } = false;
        public int RoomType { get; private set; } 
        public MarketPriceModel(decimal price, DateTime date)
        {
            Price = price;
            Date = date;
        }

        public MarketPriceModel(string hotelName, decimal price, DateTime date)
        {
            Price = price;
            Date = date;
            HotelName = hotelName;
        }

        public MarketPriceModel(decimal price, DateTime date, int roomType)
        {
            Price = price;
            Date = date;
            RoomType = roomType;
        }

    }
}

