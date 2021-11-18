using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using HotelPriceScout.Data.Model;

namespace Tests
{
    public class ComparatorTest
    {
        [Fact]
        public void ComparePricesSetsIsDiscrepancyToTrueWhenNoMarginTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "Kompas Hotel Aalborg", "tag1" },
                { "hotel2", "tag2" }
            };
            BookingSite bookingSite1 = new BookingSite("booking1", "multi", "https://www.url.com", hotelStrings);
            BookingSite bookingSite2 = new BookingSite("booking2", "multi", "https://www.url.com", hotelStrings);
            IEnumerable<BookingSite> bookingSites = new List<BookingSite>() { bookingSite1, bookingSite2 };

            int marginValue = 0;

            bookingSite1.DataScraper.StartScraping(marginValue);
            bookingSite2.DataScraper.StartScraping(marginValue);
            
            IComparator comparator = new Comparator();

            //Act
            comparator.ComparePrices(bookingSites, marginValue);

            //Assert
            Assert.True(comparator.IsDiscrepancy);            
        }

        [Fact]
        public void ComparePricesIsDiscrepancyIsFalseWhenHighMarginTest()
        {
            //Arrange
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                { "Kompas Hotel Aalborg", "tag1" },
                { "hotel2", "tag2" }
            };
            BookingSite bookingSite1 = new BookingSite("booking1", "multi", "https://www.url.com", hotelStrings);
            BookingSite bookingSite2 = new BookingSite("booking2", "multi", "https://www.url.com", hotelStrings);
            IEnumerable<BookingSite> bookingSites = new List<BookingSite>() { bookingSite1, bookingSite2 };

            int marginValue = 100;

            bookingSite1.DataScraper.StartScraping(marginValue);
            bookingSite2.DataScraper.StartScraping(marginValue);

            IComparator comparator = new Comparator();

            //Act
            comparator.ComparePrices(bookingSites, marginValue);

            //Assert
            Assert.False(comparator.IsDiscrepancy);
        }

    }
}
