using System;

namespace HotelPriceScout.Pages
{
    public class Prices : IComparable<Prices>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Prices(string _name, decimal _price)
        {
            Name = _name; 
            Price = _price;
        }
        public int CompareTo(Prices other)
        {
           return (int)(other.Price - Price);
        }
    }
}
