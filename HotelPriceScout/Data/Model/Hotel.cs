using System;

namespace HotelPriceScout.Data.Model
{
    public class Hotel
    {
        private readonly int _zipCode;

        public Hotel(string name, int zipCode)
        {
            try
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                ZipCode = zipCode;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string Name { get; }

        public int ZipCode
        {
            get => _zipCode;
            init
            {
                if (value is > 0 and < 10_000) _zipCode = value;
                else throw new ArgumentOutOfRangeException(
                    $"{nameof(value)} must be between 0 and 10.000.");
            }
        }

        private void CreateRoomType()
        {
            throw new NotImplementedException();
        }
    }
}