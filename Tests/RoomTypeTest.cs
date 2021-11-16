using System;
using Xunit;
using HotelPriceScout.Data.Model;

namespace Tests
{
    public class RoomTypeTest
    {
        [InlineData(-33)]
        [InlineData(3)]
        [InlineData(124)]
        [Theory]
        public void Test_Capacity_For_Validity_Throws(int value)
        {
            //Arrange and act
            void Action()
            {
                RoomType roomType = new RoomType(value);
            }

            Assert.Throws<ArgumentOutOfRangeException>(Action);

        }

        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [Theory]
        public void Test_Capacity_For_Validity_RoomType(int value)
        {
            //Arrange and act
            void Action(int value)
            {
                RoomType roomType = new RoomType(value);
            }

            Assert.True(Action(value));
            

        }


    }
}