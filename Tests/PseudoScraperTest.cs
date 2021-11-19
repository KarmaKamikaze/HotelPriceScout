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
            void Action()
            {
                new PseudoScraper(bookingSite);
            }

            //Act and Assert
            Assert.Throws<NullReferenceException>(Action);
        }

        [Fact]
        public void BookingsiteAssignedConstructorTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "hotel1", "tag1" }
            };
            BookingSite bookingSite = new BookingSite("bookingsite", "single", "https://url.com", hotelStrings);

            //Act
            PseudoScraper pseudoScraper = new PseudoScraper(bookingSite);

            //Assert
            Assert.Equal(bookingSite, pseudoScraper.BookingSite);


        }
    }
}