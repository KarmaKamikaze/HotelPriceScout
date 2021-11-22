using DataAccessLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace HotelPriceScout.Data.Model
{
    public class Scout
    {
        private int _marginValue;

        //This method is used to create scout objects instead of a typical constructor.
        //This is due to the fact that the static booking site data should be fetched from the database.
        //This therefore necessitates a asynchronous function.
        public static async Task<Scout> CreateScoutAsync(int marginValue, IEnumerable<DateTime> notificationTimes)
        {
            Scout scout = new Scout
            {
                MarginValue = marginValue,
                NotificationTimes = notificationTimes
            };
            SqliteDataAccess bookingSiteDB = new SqliteDataAccess();
            IEnumerable<(string, string, string, Dictionary<string, string>)> bookingSitesData = await bookingSiteDB.LoadStaticBookingSiteResources();
            scout.BookingSites = scout.CreateBookingSites(bookingSitesData);
            
            return scout;
        }

        public void StartScout()
        {
            foreach (BookingSite bookingSite in BookingSites)
            {
                bookingSite.DataScraper.StartScraping(MarginValue);
            }
        }

        public IEnumerable<DateTime> NotificationTimes { get; private set; }

        public int MarginValue
        {
            get => _marginValue;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                _marginValue = value;
            }
        }

        public IEnumerable<BookingSite> BookingSites { get; private set; }


        public void RunComparator(string type)
        {
            IComparator comparator = new Comparator();
            comparator.ComparePrices(BookingSites, MarginValue);

            /*comparator.SendNotification(); < ----ACTIVATE IF YOU WANT MAILS  */

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

        public void StopScout()
        {
            MarginValue = 0;
            NotificationTimes = null;
            BookingSites = null;
        }
    }
}