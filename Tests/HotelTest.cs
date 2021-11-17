using System;
using System.Collections.Generic;
using HotelPriceScout.Data.Model;
using Xunit;
using System.Linq;

namespace Tests
{
    public class HotelTest
    {
        [Fact]
        public void NameNullConstructorThrowsTest()
        {
            //Arrange
            string name = null;
            void Action()
            {
                new Hotel(name, "tag");
            }

            //Act and Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void NameAssignsCorrectlyConstructorTest()
        {
            //Arrange
            string name = "test";

            //Act
            Hotel hotel = new Hotel(name, "tag");

            //Assert
            Assert.Equal(name, hotel.Name);
        }

        [Fact]
        public void TagNullConstructorThrowsTest()
        {
            //Arrange
            string tag = null;
            void Action()
            {
                new Hotel("name", tag);
            }

            //Act and Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void TagAssignsCorrectlyConstructorTest()
        {
            //Arrange
            string tag = "test";

            //Act
            Hotel hotel = new Hotel("name", tag);

            //Assert
            Assert.Equal(tag, hotel.Tag);
        }


        [Fact]
        public void RoomTypesCorrectLengthConstructorTest()
        {
            //Arrange and Act
            Hotel hotel = new Hotel("name", "tag");

            //Assert
            Assert.Equal(3, hotel.RoomTypes.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public void RoomTypesAssignsCorrectCapacityConstructorTest(int capacity)
        {
            //Arrange and Act
            Hotel hotel = new Hotel("name", "tag");

            //Assert
            Assert.Equal(1, hotel.RoomTypes.Count(r => r.Capacity == capacity));
        }


    }
}
