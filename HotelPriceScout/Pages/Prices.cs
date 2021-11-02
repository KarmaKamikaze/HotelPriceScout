using System;

namespace HotelPriceScout.Pages
{
    public class Prices : IComparable<Prices>
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public Prices(string _name, int _price)
        {
            Name = _name; 
            Price = _price;
        }
        public int CompareTo(Prices other)
        {
            if (Price > other.Price) { return -1; }
            else if (Price < other.Price) { return 1; }
            else return 0;
        }
    }
}
