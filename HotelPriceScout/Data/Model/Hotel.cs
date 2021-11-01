using System;

namespace HotelPriceScout.Data.Model
{
    public class Hotel
    {
        private readonly string _name;
        private readonly int _zipCode;

        public Hotel(string name, int zipCode)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ZipCode = zipCode;
        }

        public string Name
        {
            get => _name;
            init { if(value != null) _name = value; }
        }

        public int ZipCode
        {
            get => _zipCode;
            init
            {
                if (value is > 0 and < 10_000) _zipCode = value;
            }
        }

        private void CreateRoomType()
        {
            throw new NotImplementedException();
        }
    }
}