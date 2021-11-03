using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public class Scout
    {
        private int _marginValue;
        private string _state;

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

        public string State
        {
            get => _state;
            set
            {
                if (value is not "stopped" or "started" or "preparing")
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} can not be anything other than \"stopped\", \"started\" or \"preparing\".");
                }
                _state = value;
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