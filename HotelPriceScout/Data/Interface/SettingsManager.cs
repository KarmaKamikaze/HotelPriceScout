using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Interface
{
    public class SettingsManager
    {
        public SettingsManager()
        {
            MarginDropdown = 15;
            NotificationAmountDropdown = 1;
            TimeValueDropdown = DateTime.Parse("12:00");
            TimeValue2Dropdown = DateTime.Now.Date;
            TimeValue3Dropdown = DateTime.Now.Date;
        }
        
        public static int MarginDropdown { get; set; }
        public static int NotificationAmountDropdown { get; set; }
        public static DateTime TimeValueDropdown { get; set; }
        public static DateTime TimeValue2Dropdown { get; set; }
        public static DateTime TimeValue3Dropdown { get; set; }
        public static int MarginPicked { get; private set; }
        public static int NotificationPicked { get; set; }
        private static DateTime TimeValuePicked { get; set; }
        private static DateTime TimeValue2Picked { get; set; }
        private static DateTime TimeValue3Picked { get; set; }
        public bool StartPopup { get; set; } = false;
        public bool UpdateYes { get; set; } = false;
        public static bool ShowStop { get; private set; } = false;
        public static bool ShowStart { get; private set; } = true;
        public bool StopPopup { get; set; } = false;
        public bool UpdatePopup { get; set; } = false;
        
        public IEnumerable<DateTime> GetNotificationTimes()
        {
            List<DateTime> result = new List<DateTime>();
            switch (NotificationPicked)
            {
                case 3:
                    result.Add(TimeValue3Picked);
                    goto case 2;
                case 2:
                    result.Add(TimeValue2Picked);
                    goto case 1;
                case 1:
                    result.Add(TimeValuePicked);
                    break;
                case 0:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(NotificationPicked) + "must be 0, 1, 2, or 3.");
            }
            return result;
        }
        
        public void SetScoutSettings(bool check)
        {
            if (check)
            {
                StartPopup = !StartPopup;
                ShowStop = !ShowStop;
                ShowStart = !ShowStart;
            }

            MarginPicked = MarginDropdown;
            NotificationPicked = NotificationAmountDropdown;
           
            TimeValuePicked = TimeValueDropdown;
            TimeValue2Picked = TimeValue2Dropdown;
            TimeValue3Picked = TimeValue3Dropdown;
        }
        public void SetStopScoutSettings()
        {
            ShowStart = !ShowStart;
            ShowStop = !ShowStop;
            StopPopup = !StopPopup;
            TimeValuePicked = default;
            TimeValue2Picked = default;
            TimeValue3Picked = default;
            MarginPicked = 1;
            NotificationPicked = 1;
        }
    }
}
