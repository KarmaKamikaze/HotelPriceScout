using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;


namespace HotelPriceScout.Data.Interface
{
    public class SettingsManager : ComponentBase
    {
        public static int marginPicked = 1;
        public static int notificationAmountPicked = 1;
        public static DateTime timeValue { get; set; } = DateTime.Now;
        public static DateTime timeValue2 { get; set; } = DateTime.Now;
        public static DateTime timeValue3 { get; set; } = DateTime.Now;
        public static int marginPickedPass { get; set; }
        public static int notificationPickedPass { get; set; }
        public static DateTime timeValuePass { get; set; }
        public static DateTime timeValuePass2 { get; set; }
        public static DateTime timeValuePass3 { get; set; }
        public bool isScoutStopped = false;
        public bool isScoutUpdated = false;
        public bool isScoutStarted = false;
        public bool modalStart = false;
        public bool updateYes = false;

        public IEnumerable<DateTime> GetNotificationTimes()
        {
            List<DateTime> result = new List<DateTime>();
            switch (notificationPickedPass)
            {
                case 1:
                    result.Add(timeValuePass);
                    break;
                case 2:
                    result.Add(timeValuePass);
                    result.Add(timeValuePass2);
                    break;
                case 3:
                    result.Add(timeValuePass);
                    result.Add(timeValuePass2);
                    result.Add(timeValuePass3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationPickedPass) + "must be 1, 2, or 3.");
            }

            return result;
        }

        public static bool showStop = false;
        public void ShowStopButton()
        {
            showStop = !showStop;
        }

        public static bool showStart = true;
        public void ShowStartButton()
        {
            showStart = !showStart;
        }

        public static bool showUpdate = false;
        public void ShowUpdateButton()
        {
            showUpdate = !showUpdate;
        }


        public bool popUp = false;
        public void PopUp()
        {
            popUp = !popUp;
        }

        public bool updatePopUp = false;
        public void UpdatePopUp()
        {
            updatePopUp = !updatePopUp;
        }
    }

}
