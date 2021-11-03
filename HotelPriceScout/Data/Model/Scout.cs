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

        private IEnumerable<BookingSite> CreateBookingSites(List<string[]> bookingSitesStrings)
        {
            throw new NotImplementedException();
        }


    }


}