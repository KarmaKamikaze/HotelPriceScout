using Microsoft.AspNetCore.Components;
using System;


namespace HotelPriceScout.Data.Interface
{
    public class SettingsManager : ComponentBase
    {
        public static int? marginPicked = 1;
        public static int notificationAmountPicked = 1;
        public static DateTime? timeValue { get; set; } = DateTime.Now;
        public static DateTime? timeValue2 { get; set; } = DateTime.Now;
        public static DateTime? timeValue3 { get; set; } = DateTime.Now;
        public static int? marginPickedPass { get; set; }
        public static int notificationPickedPass { get; set; }
        public static DateTime? timeValuePass { get; set; }
        public static DateTime? timeValuePass2 { get; set; }
        public static DateTime? timeValuePass3 { get; set; }
        public bool isScoutStopped = false;
        public bool isScoutUpdated = false;
        public bool isScoutStarted = false;
        public bool modalStart = false;
        public bool updateYes = false;

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

        public static bool showUdate = false;
        public void ShowUpdateButton()
        {
            showUdate = !showUdate;
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
