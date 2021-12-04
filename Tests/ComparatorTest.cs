using System.Collections.Generic;
using Xunit;
using HotelPriceScout.Data.Model;

namespace Tests
{
    public class ComparatorTest
    {
        private readonly Dictionary<string, string> _hotelStrings = new Dictionary<string, string>()
        {
            { "Kompas Hotel Aalborg", "tag1" },
            { "hotel2", "tag2" }
        };

        [Theory]
        [InlineData(0, true)]
        [InlineData(1000, false)]
        public void ComparePricesSetsIsDiscrepancyToExpectedValueTest(int marginValue, bool expected)
        {
            //Arrange
            BookingSite bookingSite1 = new BookingSite("booking1", "multi", "https://www.url.com", _hotelStrings);
            BookingSite bookingSite2 = new BookingSite("booking2", "multi", "https://www.url.com", _hotelStrings);
            IEnumerable<BookingSite> bookingSites = new List<BookingSite>() {bookingSite1, bookingSite2};

            bookingSite1.DataScraper.StartScraping(marginValue);
            bookingSite2.DataScraper.StartScraping(marginValue);

            IComparator comparator = new Comparator();

            //Act
            comparator.ComparePrices(bookingSites, marginValue);

            //Assert
            Assert.Equal(expected, comparator.IsDiscrepancy);
        }
    }
}
