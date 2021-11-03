using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public class Scout
    {
        private int _marginValue;

        public int MarginValue
        {
            get => _marginValue;
            set
            {
                if (value > 100 || value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be between 0 and 100.");
                }
                _marginValue = value;
            }
        }

        public IEnumerable<BookingSite> BookingSites { get; init; }

        //This does not work yet!!!
        private IEnumerable<BookingSite> CreateBookingSites(List<object[]> bookingSitesStrings)
        {
            List<BookingSite> bookingSites = new List<BookingSite>();
            foreach (object[] bookingSite in bookingSitesStrings)
            {
                bookingSites.Add(new BookingSite(bookingSite[0], bookingSite[1], bookingSite[2], bookingSite[3]);
            }
            
            throw new NotImplementedException();
        }


    }


}