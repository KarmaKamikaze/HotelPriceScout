using System;
namespace HotelPriceScout.Data.Interface
{
    public class Dashboard
    {
        public string monthName = "";
        public DateTime monthEnd;
        public int monthsAway = default;
        public int numDummyColumn = default;
        public int year = default;
        public int month = default;
        public int DayClicked = default;

        public void CreateMonth()
        {
            var tempDate = DateTime.Now.AddMonths(monthsAway);
            month = tempDate.Month;
            year = tempDate.Year;

            DateTime monthStart = new DateTime(year, month, 1);
            monthEnd = monthStart.AddMonths(1).AddDays(-1);
            monthName = monthStart.Month switch
            {
                1 => "Januar",
                2 => "Februar",
                3 => "Marts",
                4 => "April",
                5 => "Maj",
                6 => "Juni",
                7 => "Juli",
                8 => "August",
                9 => "September",
                10 => "Oktober",
                11 => "November",
                12 => "December",
                _ => ""
            };

            numDummyColumn = (int)monthStart.DayOfWeek;

            if(numDummyColumn == 0)
            {
                numDummyColumn = 7;
            }
        }
        public void ShowMoreInfo(int DayClicked)
        {
            if (DayClicked == this.DayClicked)
            {
                this.DayClicked = 0;
            }
            else
            {
                this.DayClicked = DayClicked;
            }
        }
    }
}

