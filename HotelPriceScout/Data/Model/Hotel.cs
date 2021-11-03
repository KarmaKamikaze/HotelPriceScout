using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public class Hotel
    {
        public Hotel(string name)
        {
            try
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                RoomTypes = CreateRoomTypes();

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string Name { get; }

        public IEnumerable<RoomType> RoomTypes { get; init; }

        private IEnumerable<RoomType> CreateRoomTypes()

        {
            return new List<RoomType>
            {
                new RoomType(1),
                new RoomType(2),
                new RoomType(4)
            };
        }
    }
}