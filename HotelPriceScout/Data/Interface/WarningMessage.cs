namespace HotelPriceScout.Data.Interface
{
    public class WarningMessage
    {
        public string BookingSite { get; }
        public string ConcatenatedWarningString { get; }

        public WarningMessage(string concatenatedWarning, string bookingSite)
        {
            ConcatenatedWarningString = concatenatedWarning;
            BookingSite = bookingSite;
        }
    }
}
