using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public class Hotel
    {
        public Hotel(string name, string tag)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            RoomTypes = CreateRoomTypes();
        }

        public string Tag { get; }

        public string Name { get; }

        public IEnumerable<RoomType> RoomTypes { get; }

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
