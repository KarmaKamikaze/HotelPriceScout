using HotelPriceScout.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Components.Web;

namespace Tests
{
    public class SettingsManagerTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        [InlineData(500)]
        public void Test_If_GetNotificationTimes_Throws_Error(int notificationValue)
        {
            //Arrange and Act
            SettingsManager settingsManager = new SettingsManager();
            SettingsManager.NotificationPicked = notificationValue;

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(settingsManager.GetNotificationTimes);
        }

        public void Test_If_SetStartScoutSettings_Reverses_Bools()
        {

        }

        public void Test_If_SetStopScoutSettings_Reverses_Bools()
        {

        }

        public void Test_If_SetUpdateScoutSettings_Reverses_Bools()
        {

        }


        public static readonly object[][] EscapeUpdateInfo =
        {
            new object[] {},
            new object[] {},
            new object[] {}
        };

        
        public void Test_If_EscapeUpdate_Escapes()
        {

        }

        public void Test_If_EscapeStop_Escapes()
        {

        }





    }
}
