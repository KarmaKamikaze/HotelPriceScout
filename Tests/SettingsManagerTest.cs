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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Test_If_GetNotificationTimes_Is_Correct_Length(int NotificationAmount)
        {
            //Arrange and Act
            SettingsManager settingsManager = new SettingsManager();
            SettingsManager.NotificationPicked = NotificationAmount;

            //Assert
            Assert.Equal(NotificationAmount, settingsManager.GetNotificationTimes().Count());
        }
    }
}
