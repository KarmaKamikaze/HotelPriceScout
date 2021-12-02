namespace HotelPriceScout.Data.Interface
{
    public class WarningMessage
    {
        public string BookingSite { get; set; }
        public string ConcatenatedWarningString { get; set; }

        public WarningMessage(string concatenatedWarning, string bookingSite)
        {
            ConcatenatedWarningString = concatenatedWarning;
            BookingSite = bookingSite;
        }
    }
}
