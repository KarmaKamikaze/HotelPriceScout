using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{
    public class Scout
    {
        private int _marginValue;
        private string _state;

        public Scout(string state, int marginValue, DateTime[] notificationTimes)
        {
            State = state;
            MarginValue = marginValue;
            NotificationTimes = notificationTimes;
            //BookingSites = CreateBookingSites(/*This should be the static method that return the booking site information from the database*/);
        }

        public DateTime[] NotificationTimes { get; init; }

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
        private IEnumerable<BookingSite> CreateBookingSites(List<(string name, string type, string url, List<string> hotels)> bookingSitesStrings)
        {
            List<BookingSite> bookingSites = new();
            foreach ((string name, string type, string url, List<string> hotels) bookingSite in bookingSitesStrings)
            {
                bookingSites.Add(new BookingSite(bookingSite.name, bookingSite.type, bookingSite.url, bookingSite.hotels));
            }

            throw new NotImplementedException();
        }


    }


}