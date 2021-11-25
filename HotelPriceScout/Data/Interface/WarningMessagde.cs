using System.Collections.Generic;
namespace HotelPriceScout.Data.Interface
{
    public class WarningMessagde
    {
        private string bookingSite { get; set; }
        private string listOfWarnings { get; set; }
        public string BookingSite { get { return bookingSite; } set { bookingSite = value; } }
        public string ListofWarnings { get { return listOfWarnings; } set { listOfWarnings = value; } }

        public WarningMessagde( string listOfWarnings, string bookingSite)
        {
            this.ListofWarnings = listOfWarnings;
            this.BookingSite = bookingSite;
        }



    }
}
