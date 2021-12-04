using System;

namespace DataAccessLibrary
{
    public class PriceModel : IComparable<PriceModel> 
    {
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
        
        public decimal Price { get; }
        public DateTime Date { get; }
        public string HotelName { get; }
        public bool MarkedForDiscrepancy { get; set; }
        public int RoomType { get; } 
        
        public int CompareTo(PriceModel other)
        {
            return (decimal.ToInt32(other.Price) - decimal.ToInt32(Price));
        }
    }
}

