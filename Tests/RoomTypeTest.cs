using System;
using Xunit;
using HotelPriceScout.Data.Model;
using System.Linq;

namespace Tests
{
    public class RoomTypeTest
    {
        [Theory]
        [InlineData(-33)]
        [InlineData(3)]
        [InlineData(124)]
        public void Test_Capacity_For_Validity_Throws(int value)
        {
            //Arrange and act
            void Action()
            {
                RoomType roomType = new RoomType(value);
            }

            Assert.Throws<ArgumentOutOfRangeException>(Action);

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public void Test_If_Capacity_Accepts_Correct_Input(int value)
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

            //Makes sure that if there's a year change, it'll + the days to the total amount.
            // It says 366, as the days of the year range from 1-366 instead of 0-365.
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
            //Arrange and Act
            var value = DateTime.Now.Date;
            RoomType roomType = new RoomType(1);

            //Assert
            Assert.Equal(roomType.Prices.Min(price => price.Date), value);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public void Test_For_No_Null_Prices(int value)
        {
            //Arrange and Act
            RoomType roomType = new RoomType(value);

            //Assert
            Assert.DoesNotContain(null, roomType.Prices);
        }
    }
}