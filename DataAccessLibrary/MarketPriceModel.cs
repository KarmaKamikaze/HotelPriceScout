using System;
namespace HotelPriceScout.Data.Model
{
    public class MarketPriceModel
    {
        public int Price { get; set; }
        public DateTime Date { get; set; }

        public MarketPriceModel(int price, DateTime date)
        {
            Price = price;
            Date = date;

        }
    }
}

