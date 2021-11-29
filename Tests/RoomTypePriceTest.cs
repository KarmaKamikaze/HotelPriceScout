using System;
using HotelPriceScout.Data.Model;
using Xunit;

namespace Tests
{
    public class RoomTypePriceTest
    {
        [Fact]
        public void DateSetterThrowsWhenValueIsBeforeCurrentDateTest()
        {
            //Arrange
            DateTime date = DateTime.Now.AddDays(-1);

            void Action() => new RoomTypePrice(date);

            //Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(Action);
        }

        [Fact]
        public void DateSetterCorrectlyAssignsValueTest()
        {
            //Arrange
            DateTime date = DateTime.Now;

            //Act
            RoomTypePrice roomTypePrice = new RoomTypePrice(date);

            //Assert
            Assert.Equal(date, roomTypePrice.Date);
        }

        [Fact]
        public void PriceSetterThrowsWhenValueIsNegativeTest()
        {
            //Arrange
            DateTime date = DateTime.Now;
            decimal price = -1;

            void Action() => new RoomTypePrice(date, price);

            //Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(Action);
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
    }
}