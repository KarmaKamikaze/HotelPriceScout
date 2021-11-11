using System;
namespace HotelPriceScout.Data.Model
{
    public class MarketPriceModel
    {
        public int Price { get; set; }
        public DateTime Date { get; set; }
        public string HotelName { get; set;  }
        public bool MarkedForDiscrepancy { get; set; } = false;
        public int RoomType { get; private set; } 
        public MarketPriceModel(int price, DateTime date)
        {
            Price = price;
            Date = date;
        }
        public MarketPriceModel(string hotelname, int price, DateTime date)
        {
            Price = price;
            Date = date;
            HotelName = hotelname;
        }
        public MarketPriceModel(int price, DateTime date, int roomType)
        {
            Price = price;
            Date = date;
            RoomType = roomType;
        }

    }
}

