using DataAccessLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;


namespace HotelPriceScout.Data.Model
{
    public class Scout
    {
        private int _marginValue;
        private string _state;

        //This method is used to create scout objects instead of a typical constructor.
        //This is due to the fact that the static booking site data should be fetched from the database.
        //This therefore necessitates a asynchronous function.
        public static async Task<Scout> CreateScoutAsync(string state, int marginValue, DateTime[] notificationTimes)
        {
            Scout scout = new Scout();
            scout.State = state;
            scout.MarginValue = marginValue;
            scout.NotificationTimes = notificationTimes;
            SqliteDataAccess bookingSiteDB = new SqliteDataAccess();
            IEnumerable<(string, string, string, Dictionary<string, string>)> bookingSitesData = await bookingSiteDB.LoadStaticBookingSiteResources();
            scout.BookingSites = scout.CreateBookingSites(bookingSitesData);
            
            return scout;
        }

        public DateTime[] NotificationTimes { get; private set; }

        public int MarginValue
        {
            get => _marginValue;
            private set
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
                if (value != "stopped" && value != "started" && value != "preparing")
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} can not be anything other than \"stopped\", \"started\" or \"preparing\".");
                }
                _state = value;
            }
        }

        public IEnumerable<BookingSite> BookingSites { get; private set; }


        public void RunComparator(string type)
        {
            Comparator comparator = new Comparator();
            comparator.ComparePrices(BookingSites, MarginValue);

            if (comparator.IsDiscrepancy && type == "email")
            {
                comparator.SendNotification();
            }

        }


        private IEnumerable<BookingSite> CreateBookingSites(IEnumerable<(string, string, string, Dictionary<string, string>)> bookingSitesData)
        {
            List<BookingSite> bookingSites = new();
            foreach ((string name, string type, string url, Dictionary<string, string> hotels) in bookingSitesData)
            {
                bookingSites.Add(new BookingSite(name, type, url, hotels));
            }

            return bookingSites;
        }
    }
}