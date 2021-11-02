using System;

namespace HotelPriceScout.Data.Model
{
    public class Hotel
    {
        public Hotel(string name)
        {
            try
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string Name { get; }

        private void CreateRoomType()
        {
            throw new NotImplementedException();
        }
    }
}