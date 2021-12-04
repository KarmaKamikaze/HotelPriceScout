using HotelPriceScout.Data.Interface;
using System;
using System.Linq;
using Xunit;

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
        public void Test_If_GetNotificationTimes_Is_Correct_Length(int notificationAmount)
        {
            //Arrange and Act
            SettingsManager settingsManager = new SettingsManager();
            SettingsManager.NotificationPicked = notificationAmount;

            //Assert
            Assert.Equal(notificationAmount, settingsManager.GetNotificationTimes().Count());
        }
    }
}