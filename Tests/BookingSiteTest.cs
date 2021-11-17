using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using HotelPriceScout.Data.Model;

namespace Tests
{
    public class BookingSiteTest
    {
        [Fact]
        public void NameNullConstructorThrowsTest()
        {
            //Arrange
            string name = null;
            void Action()
            {
                new BookingSite(name, "single", "url", new Dictionary<string, string>());
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
            BookingSite bookingSite = new BookingSite(name, "single", "url", new Dictionary<string, string>());

            //Assert
            Assert.Equal(name, bookingSite.Name);
        }

        [Fact]
        public void URLNullConstructorThrowsTest()
        {
            //Arrange
            string url = null;
            void Action()
            {
                new BookingSite("name", "single", url, new Dictionary<string, string>());
            }

            //Act and Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void URLAssignsCorrectlyConstructorTest()
        {
            //Arrange
            string url = "test";

            //Act
            BookingSite bookingSite = new BookingSite("name", "single", url, new Dictionary<string, string>());

            //Assert
            Assert.Equal(url, bookingSite.Url);
        }

        [Fact]
        public void TypeNullConstructorThrowsTest()
        {
            //Arrange
            string type = null;
            void Action()
            {
                new BookingSite("name", type, "url", new Dictionary<string, string>());
            }

            //Act and Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void TypeSetterThrowsWhenValueOutOfRangeTest()
        {
            //Arrange
            string type = "test";
            void Action()
            {
                new BookingSite("name", type, "url", new Dictionary<string, string>());
            }

            //Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(Action);
        }

        [Theory]
        [InlineData("single")]
        [InlineData("multi")]
        public void TypeSetterAssignsCorrectlyTest(string type)
        {
            //Arrange and Act
            BookingSite bookingSite = new BookingSite("name", type, "url", new Dictionary<string, string>());

            //Assert
            Assert.Equal(type, bookingSite.Type);
        }

        [Fact]
        public void HotelsListCorrectLengthTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "hotel1", "tag1" },
                { "hotel2", "tag2" },
                { "hotel3", "tag3" }
            };
            int expected = 3;

            //Act
            BookingSite bookingSite = new BookingSite("name", "multi", "url", hotelStrings);

            //Assert
            Assert.Equal(expected, bookingSite.HotelsList.Count());
        }

        [Fact]
        public void HotelsListIsEmptyWhenDictionaryIsEmptyTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>();
            int expected = 0;

            //Act
            BookingSite bookingSite = new BookingSite("name", "multi", "url", hotelStrings);

            //Assert
            Assert.Equal(expected, bookingSite.HotelsList.Count());
        }

        [Fact]
        public void HotelsListOnlyContainsDataFromTheDictionaryTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "hotel1", "tag1" },
                { "hotel2", "tag2" },
                { "hotel3", "tag3" }
            };

            //Act
            BookingSite bookingSite = new BookingSite("name", "multi", "url", hotelStrings);

            //Assert
            Assert.All(bookingSite.HotelsList, hotel => Assert.True(hotelStrings.Keys.Contains(hotel.Name)
                && hotelStrings[hotel.Name] == hotel.Tag));
        }

        [Fact]
        public void HotelsListOnlyContainsUniqueHotelNamesTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "hotel1", "tag1" },
                { "hotel2", "tag2" },
                { "hotel3", "tag3" }
            };

            //Act
            BookingSite bookingSite = new BookingSite("name", "multi", "url", hotelStrings);

            //Assert
            Assert.All(bookingSite.HotelsList, hotel => Assert.True(bookingSite.HotelsList.Where(h => h.Name == hotel.Name).Count() == 1));
        }
    }
}
