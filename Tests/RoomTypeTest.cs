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
        public void Test_Capacity_For_Validity_RoomType_One(int value)
        {
            //Arrange and Act
            RoomType roomType = new RoomType(value);

            //Assert
            Assert.Equal(value, roomType.Capacity);
        }

        [Fact]
        public void Test_Length_Of_Prices_List()
        {
            //Arrange
            int value = ((DateTime.Now.AddMonths(3).DayOfYear) - DateTime.Now.DayOfYear);

            //Makes sure that if there's a year change, it'll + the days to the total amount
            if(value < 0)
            {
                value = value + 366;
            }

            RoomType roomType = new RoomType(1);

            Assert.Equal(roomType.Prices.Count, value);
        }

        [Fact]
        public void Test_For_Minimum_Date()
        {
            //Arrange

            //Act

            //Assert
        }

    }
}