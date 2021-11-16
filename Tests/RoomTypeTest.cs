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
        [InlineData(5)]
        [Theory]
        public void Test_Capacity_For_Validity_RoomType_One(int value)
        {
            //Arrange
            DateTime date = DateTime.Now;

            //Act
            RoomType Capacity = new RoomType(capacity);

            //Assert
            Assert.Equal(value, capacity);
        }
    }
}


[Theory]
[InlineData(0)]
[InlineData(1)]
public void PriceSetterCorrectlyAssignsValueTest(decimal price)
{
    //Arrange
    DateTime date = DateTime.Now;

    //Act
    RoomTypePrice roomTypePrice = new RoomTypePrice(date, price);

    //Assert
    Assert.Equal(price, roomTypePrice.Price);
}