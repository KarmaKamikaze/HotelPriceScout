using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;


namespace HotelPriceScout.Data.Interface
{
    public class SettingsManager : ComponentBase
    {
        public static int marginPicked = 1;
        public static int notificationAmountPicked = 1;
        public static DateTime timeValue { get; set; } = DateTime.Now.Date;
        public static DateTime timeValue2 { get; set; } = DateTime.Now.Date;
        public static DateTime timeValue3 { get; set; } = DateTime.Now.Date;
        public static int marginPickedPass { get; set; }
        public static int notificationPickedPass { get; set; }
        public static DateTime timeValuePass { get; set; }
        public static DateTime timeValuePass2 { get; set; }
        public static DateTime timeValuePass3 { get; set; }
        public bool modalStart = false;
        public bool updateYes = false;
        public static bool showStop = false;
        public static bool showStart = true;
        public static bool showUpdate = false;
        public bool popUp = false;
        public bool updatePopUp = false;

        public void PopUp()
        {
            popUp = !popUp;
        }
        public void UpdatePopUp()
        {
            updatePopUp = !updatePopUp;
        }
        public void ModalStart()
        {
            modalStart = !modalStart;
        }

        public void ReverseBool(ref bool i)
        {
            i = !i;
        }

        public IEnumerable<DateTime> GetNotificationTimes()
        {
            List<DateTime> result = new List<DateTime>();
            switch (notificationPickedPass)
            {
                case 3:
                    result.Add(timeValuePass3);
                    goto case 2;
                case 2:
                    result.Add(timeValuePass2);
                    goto case 1;
                case 1:
                    result.Add(timeValuePass);
                    break;
                case 0:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationPickedPass) + "must be 0, 1, 2, or 3.");
            }

            return result;
        }

        public class NotificationAmount
        {
            public int Notification { get; set; }
            public string Text { get; set; }
        }
        public List<NotificationAmount> Notifications = new List<NotificationAmount>()
        {
        new NotificationAmount(){ Notification= 0, Text= "0" },
        new NotificationAmount(){ Notification= 1, Text= "1" },
        new NotificationAmount(){ Notification= 2, Text= "2" },
        new NotificationAmount(){ Notification= 3, Text= "3" },
        };

        public void SetStartScoutSettings()
        {
            ReverseMultipleBools(ref modalStart, ref showStop, ref showUpdate, ref showStart);

            marginPickedPass = marginPicked;
            notificationPickedPass = notificationAmountPicked;
            timeValuePass = timeValue;
            timeValuePass2 = timeValue2;
            timeValuePass3 = timeValue3;
        }

        public void SetStopScoutSettings()
        {
            ReverseMultipleBools(ref showStart, ref showUpdate, ref showStop, ref popUp);
            marginPickedPass = default;
            notificationPickedPass = default;
            timeValuePass = default;
            timeValuePass2 = default;
            timeValuePass3 = default;
            marginPicked = 1;
            notificationAmountPicked = 1;
        }

        public void SetUpdateScoutSettings()
        {
            marginPickedPass = marginPicked;
            notificationPickedPass = notificationAmountPicked;
            timeValuePass = timeValue;
            timeValuePass2 = timeValue2;
            timeValuePass3 = timeValue3;
            ReverseBool(ref updatePopUp);
        }

        /*Reverse bools for StartProgram/StopProgram*/
        public static void ReverseMultipleBools(ref bool a, ref bool b, ref bool c, ref bool d)
        {
            a = !a;
            b = !b;
            c = !c;
            d = !d;
        }
    }
}