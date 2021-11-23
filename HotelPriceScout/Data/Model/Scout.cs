using DataAccessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using HotelPriceScout.Data.Function;


namespace HotelPriceScout.Data.Model
{
    public class Scout : IScout
    {
        private int _marginValue;

        private int MarginValue
        {
            get => _marginValue;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                _marginValue = value;
            }
        }
        private IEnumerable<DateTime> NotificationTimes { get; set; }
        private TimeMonitor Timers { get; set; }
        public IEnumerable<BookingSite> BookingSites { get; private set; }
        
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
            scout.UpdateTimeDetermination();
            
            return scout;
        }

        public void StartScout()
        {
            foreach (BookingSite bookingSite in BookingSites)
            {
                bookingSite.DataScraper.StartScraping(MarginValue);
            }
        }
        
        public void StopScout()
        {
            MarginValue = 0;
            NotificationTimes = null;
            BookingSites = null;
        }

        public void RunComparator(string type)
        {
            IComparator comparator = new Comparator();
            comparator.ComparePrices(BookingSites, MarginValue);

            if (comparator.IsDiscrepancy && type == "email")
            {
                comparator.SendNotification();   
            }
        }

        private void UpdateTimeDetermination()
        {
            Timers = new TimeMonitor(NotificationTimes, OnTimeToNotify);
        }

        private void OnTimeToNotify(object sender, ElapsedEventArgs eventArgs)
        {
            RunComparator("email");
            UpdateTimeDetermination();
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