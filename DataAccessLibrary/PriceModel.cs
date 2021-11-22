using System;

namespace HotelPriceScout.Data.Model
{
    public class PriceModel : IComparable<PriceModel> 
    {
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public string HotelName { get; set;  }
        public bool MarkedForDiscrepancy { get; set; } = false;
        public int RoomType { get; private set; } 
        public PriceModel(decimal price, DateTime date)
        {
            Price = price;
            Date = date;
        }

        public PriceModel(string hotelName, decimal price, DateTime date)
        {
            Price = price;
            Date = date;
            HotelName = hotelName;
        }

        public PriceModel(decimal price, DateTime date, int roomType)
        {
            Price = price;
            Date = date;
            RoomType = roomType;
        }
        public PriceModel(decimal price, string hotelName)
        {
            Price = price;
            HotelName = hotelName;
        }
        public int CompareTo(PriceModel other)
        {
            return (decimal.ToInt32(other.Price) - decimal.ToInt32(Price));
        }
    }
}

