using System;
namespace HotelPriceScout.Data.Model
{
    public class MarketPriceModel
    {
        public int Price { get; set; }
        public DateTime Date { get; set; }
        public string HotelName { get; set;  }
        public MarketPriceModel(int price, DateTime date)
        {
            Price = price;
            Date = date;
        }
        public MarketPriceModel(int price, DateTime date, string hotelname)
        {
            Price = price;
            Date = date;
            HotelName = hotelname;
        }
    }
}

