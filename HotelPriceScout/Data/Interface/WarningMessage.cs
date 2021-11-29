using System.Collections.Generic;
namespace HotelPriceScout.Data.Interface
{
    public class WarningMessage
    {
      
        public string BookingSite { get; set; }
        public string ListofWarnings { get; set; }

        public WarningMessage( string listOfWarnings, string bookingSite)
        {
            this.ListofWarnings = listOfWarnings;
            this.BookingSite = bookingSite;
        }



    }
}
