using System;
using IronWebScraper;

namespace HotelPriceScout.Data
{
    public class DataScraper : WebScraper
    {
        public DataScraper(string hotelName)
        {
            //Test parameters underneath:
            ArrivalDate = new DateTime(2021,11,15);
            NumOfAdults = 2;
            //Real parameters underneath:
            //TODO: Make sure that ArrivalDate and NumOfAdults are not null, so that the right starting url will be set.
            Url = hotelName;
        }
        
        private string _url;
        private DateTime _arrivalDate;
        private int _numOfAdults;

        public string Url
        {
            get => _url;
            set
            {
                if (value != null)
                {
                    _url = SetHotelUrl(value);
                }
            }
        }

        public DateTime ArrivalDate
        {
            get => _arrivalDate;
            set => _arrivalDate = value;
        }

        public int NumOfAdults
        {
            get => _numOfAdults;
            set
            {
                if (value is < 5 and > 0 and not 3)
                {
                    _numOfAdults = value;
                }
            }
        }

        private string SetHotelUrl(string hotelName)
        {
            switch (hotelName)
            {
                case "Kompas":
                {
                    return $"https://hotelaalborg.bookingportal.net/booking/room/1?HotelCode=HAA&ArrivalDate={ArrivalDate:yyyy-MM-dd}&LengthOfStay=1&RoomCounts={NumOfAdults.ToString()},0,0,0,0,0&Single=true&BookingFlow=1&RateCode=";
                }
                default:
                    throw new Exception("SetHotelUrl() did not receive a valid hotelName.");
            }
        }


        public override void Init()
        {
            this.LoggingLevel = WebScraper.LogLevel.All;
            this.WorkingDirectory = AppContext.BaseDirectory.Replace("\\bin\\Debug", "\\Data\\Output");
            EnableWebCache(new TimeSpan(1, 30, 30));
            this.Request(Url, Parse);
        }

        public override void Parse(Response response)
        {
            foreach (var titleLink in response.Css("footer > div.footer > div.row > div.text-center > div.text-left."))
            {
                string strTitle = titleLink.TextContentClean;
                Scrape(new ScrapedData() { { "Title", strTitle } });
            }

            if (response.CssExists("div.prev-post > a[href]"))
            {
                var nextPage = response.Css("div.prev-post > a[href]")[0].Attributes["href"];
                this.Request(nextPage, Parse);
            }
        }
    }
}