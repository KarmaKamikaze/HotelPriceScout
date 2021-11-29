using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Interface
{
    public class SettingsManager
    {
        public static int MarginDropdown { get; set; } = 15;
        public static int NotificationAmountDropdown { get; set; } = 1;
        public static DateTime TimeValueDropdown { get; set; } = DateTime.Parse("12:00");
        public static DateTime TimeValue2Dropdown { get; set; } = DateTime.Now.Date;
        public static DateTime TimeValue3Dropdown { get; set; } = DateTime.Now.Date;
        public static int MarginPicked { get; private set; }
        public static int NotificationPicked { get; set; }
        private static DateTime TimeValuePicked { get; set; }
        private static DateTime TimeValue2Picked { get; set; }
        private static DateTime TimeValue3Picked { get; set; }
        public bool startPopup = false;
        public bool updateYes = false;
        public static bool showStop = false;
        public static bool showStart = true;
        public bool stopPopup = false;
        public bool updatePopup = false;
        
        public void ModalStopPopUp()
        {
            stopPopup = !stopPopup;
        }
        public void ModalUpdatePopUp()
        {
            updatePopup = !updatePopup;
        }
        public void ModalStartPopUp()
        {
            startPopup = !startPopup;      
        }

        private static void ReverseMultipleBools(ref bool a, ref bool b, ref bool c)
        {
            a = !a;
            b = !b;
            c = !c;
        }
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
        
        public void SetStartScoutSettings()
        {
            ReverseMultipleBools(ref startPopup, ref showStop, ref showStart);
            MarginPicked = MarginDropdown;
            NotificationPicked = NotificationAmountDropdown;
           
            TimeValuePicked = TimeValueDropdown;
            TimeValue2Picked = TimeValue2Dropdown;
            TimeValue3Picked = TimeValue3Dropdown;
        }
        public void SetStopScoutSettings()
        {
            ReverseMultipleBools(ref showStart, ref showStop, ref stopPopup);
            MarginPicked = default;
            NotificationPicked = default;
            TimeValuePicked = default;
            TimeValue2Picked = default;
            TimeValue3Picked = default;
            MarginPicked = 1;
            NotificationPicked = 1;
        }
        public void SetUpdateScoutSettings()
        {
            MarginPicked = MarginDropdown;
            NotificationPicked = NotificationAmountDropdown;
            TimeValuePicked = TimeValueDropdown;
            TimeValue2Picked = TimeValue2Dropdown;
            TimeValue3Picked = TimeValue3Dropdown;
            updatePopup = !updatePopup;

        }
        public void EscapeUpdate(KeyboardEventArgs e)
        {
            if (e.Code == "Escape" && updatePopup)
            {
                updatePopup = !updatePopup;
            }
        }
        public void EscapeStop(KeyboardEventArgs f)
        {
            if (f.Code == "Escape" && stopPopup)
            {
                stopPopup = !stopPopup;
            }
        }
    }
}
