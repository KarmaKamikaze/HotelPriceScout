using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{

    public class Comparator
    {
        private readonly string _type;

        public Comparator(string type, IEnumerable<BookingSite> bookingSites, int marginValue)
        {
            Type = type;
            BookingSites = bookingSites;
        }

        private bool IsDiscrepancy { get; set; }

        public string Type
        {
            get => _type;
            init
            {
                if (value is "dashboard" or "email") _type = value;
                else throw new ArgumentOutOfRangeException(
                    $"{nameof(value)} must be either \"dashboard\" or \"email\".");
            }
        }

        public IEnumerable<BookingSite> BookingSites { get; init; }

        public void ComparePrices()
        {
            throw new NotImplementedException();
        }

        public void SendNotification()
        {
            throw new NotImplementedException();
        }

    }





}
