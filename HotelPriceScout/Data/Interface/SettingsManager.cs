using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using IronWebScraper;
using Request = Org.BouncyCastle.Asn1.Ocsp.Request;


namespace HotelPriceScout.Data.Interface
{
    public class SettingsManager
    {
        public static int MarginDropdown { get; set; } = 15;
        public static int NotificationAmountDropdown { get; set; } = 1;
        public static DateTime TimeValueDropdown { get; set; } = DateTime.Now.Date;
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

        public void ReverseBool(ref bool i)
        {
            i = !i;
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
        public class NotificationAmount
        {
            public int Notification { get; set; }
            public string Text { get; set; }
        }
        public readonly List<NotificationAmount> notifications = new List<NotificationAmount>()
        {
        new NotificationAmount(){ Notification= 0, Text= "0" },
        new NotificationAmount(){ Notification= 1, Text= "1" },
        new NotificationAmount(){ Notification= 2, Text= "2" },
        new NotificationAmount(){ Notification= 3, Text= "3" },
        };
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
            ReverseBool(ref updatePopup);

        }
        public void EscapeUpdate(KeyboardEventArgs e)
        {
            if (e.Code == "Escape" && updatePopup)
            {
                ReverseBool(ref updatePopup);
            }
        }
        public void EscapeStop(KeyboardEventArgs f)
        {
            if (f.Code == "Escape" && stopPopup)
            {
                ReverseBool(ref stopPopup);
            }
        }
    }
}
