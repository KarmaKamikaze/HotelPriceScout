using System;
using System.Collections.Generic;
using HotelPriceScout.Data.Function;
using HotelPriceScout.Data.Model;
using Xunit;

namespace Tests
{
    public class PseudoScraperTest
    {
        [Fact]
        public void BookingsiteNullConstructorThrowsTest()
        {
            //Arrange
            BookingSite bookingSite = null;

            void Action() => new PseudoScraper(bookingSite);

            //Act and Assert
            Assert.Throws<NullReferenceException>(Action);
        }

        [Fact]
        public void ValidBookingSiteAssignedConstructorTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "hotel1", "tag1" }
            };
            BookingSite bookingSite = new BookingSite("bookingsite", "single", "https://url.com", hotelStrings);

            //Act
            IDataScraper pseudoScraper = new PseudoScraper(bookingSite);

            //Assert
            Assert.Equal(bookingSite, pseudoScraper.BookingSite);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void StartScrapingAssignsPricesTest(decimal margin)
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "hotel1", "tag1" }
            };
            BookingSite bookingSite = new BookingSite("bookingsite", "single", "https://url.com", hotelStrings);
            IDataScraper pseudoScraper = new PseudoScraper(bookingSite);

            //Act
            pseudoScraper.StartScraping(margin);

            //Assert
            Assert.All(bookingSite.HotelsList,
                hotel => Assert.All(hotel.RoomTypes,
                    roomType => Assert.All(roomType.Prices,
                        price => Assert.NotEqual(0, price.Price))));
        }
    }
}
