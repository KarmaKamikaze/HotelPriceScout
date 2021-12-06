using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using HotelPriceScout.Data.Model;

namespace Tests
{
    public class BookingSiteTest
    {
        private readonly Dictionary<string, string> _hotelStrings = new Dictionary<string, string>()
        {
            { "hotel1", "tag1" },
            { "hotel2", "tag2" },
            { "hotel3", "tag3" }
        };
        
        
        [Fact]
        public void NameNullConstructorThrowsTest()
        {
            //Arrange
            string name = null;

            void Action() => new BookingSite(name, "single", "https://www.url.com", _hotelStrings);

            //Act and Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void NameAssignsCorrectlyConstructorTest()
        {
            //Arrange
            string name = "test";

            //Act
            BookingSite bookingSite =
                new BookingSite(name, "single", "https://www.url.com", _hotelStrings);

            //Assert
            Assert.Equal(name, bookingSite.Name);
        }

        [Fact]
        public void URLNullConstructorThrowsTest()
        {
            //Arrange
            string url = null;

            void Action() => new BookingSite("name", "single", url, _hotelStrings);

            //Act and Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("www.test.com")]
        public void URLThrowsWhenWrongFormatTest(string url)
        {
            //Arrange
            void Action() => new BookingSite("name", "single", url, _hotelStrings);

            //Act and Assert
            Assert.Throws<UriFormatException>(Action);
        }

        [Fact]
        public void URLAssignsCorrectlyConstructorTest()
        {
            //Arrange
            string url = "https://www.test.com";

            //Act
            BookingSite bookingSite = new BookingSite("name", "single", url, _hotelStrings);

            //Assert
            Assert.Equal(url, bookingSite.Url);
        }

        [Fact]
        public void TypeNullConstructorThrowsTest()
        {
            //Arrange
            string type = null;

            void Action() => new BookingSite("name", type, "https://www.url.com", _hotelStrings);

            //Act and Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void TypeSetterThrowsWhenValueOutOfRangeTest()
        {
            //Arrange
            string type = "test";

            void Action() => new BookingSite("name", type, "https://www.url.com", _hotelStrings);

            //Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(Action);
        }

        [Theory]
        [InlineData("single")]
        [InlineData("multi")]
        public void TypeSetterAssignsCorrectlyTest(string type)
        {
            //Arrange and Act
            BookingSite bookingSite =
                new BookingSite("name", type, "https://www.url.com", _hotelStrings);

            //Assert
            Assert.Equal(type, bookingSite.Type);
        }

        [Fact]
        public void HotelsListCorrectLengthTest()
        {
            //Arrange
            int expected = 3;

            //Act
            BookingSite bookingSite = new BookingSite("name", "multi", "https://www.url.com", _hotelStrings);

            //Assert
            Assert.Equal(expected, bookingSite.HotelsList.Count());
        }

        [Fact]
        public void HotelsListIsEmptyWhenDictionaryIsEmptyTest()
        {
            //Arrange
            Dictionary<string, string> emptyHotelStrings = new Dictionary<string, string>();
            int expected = 0;

            //Act
            BookingSite bookingSite = new BookingSite("name", "multi", "https://www.url.com", emptyHotelStrings);

            //Assert
            Assert.Equal(expected, bookingSite.HotelsList.Count());
        }

        [Fact]
        public void HotelsListOnlyContainsDataFromTheDictionaryTest()
        {
            //Arrange and Act
            BookingSite bookingSite = new BookingSite("name", "multi", "https://www.url.com", _hotelStrings);

            //Assert
            Assert.All(bookingSite.HotelsList, hotel => Assert.True(_hotelStrings.Keys.Contains(hotel.Name)
                                                                    && _hotelStrings[hotel.Name] == hotel.Tag));
        }

        [Fact]
        public void HotelsListOnlyContainsUniqueHotelNamesTest()
        {
            //Arrange and Act
            BookingSite bookingSite = new BookingSite("name", "multi", "https://www.url.com", _hotelStrings);

            //Assert
            Assert.All(bookingSite.HotelsList,
                hotel => Assert.True(bookingSite.HotelsList.Count(h => h.Name == hotel.Name) == 1));
        }
    }
}
