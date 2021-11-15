using System;

namespace HotelPriceScout.Data.Model
{
    public class MarketPriceModel
    {
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public string HotelName { get; set;  }
        public MarketPriceModel(decimal price, DateTime date)
        {
            Price = price;
            Date = date;
        }
        public MarketPriceModel(string hotelname, decimal price, DateTime date)
        {
            Price = price;
            Date = date;
            HotelName = hotelname;
        }
    }
}

