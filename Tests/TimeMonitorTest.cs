using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Xunit;
using HotelPriceScout.Data.Function;

namespace Tests
{
    public class TimeMonitorTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(100)]
        public void TimeKeepersCorrectCountTest(int count)
        {
            //Arrange
            IEnumerable<DateTime> notificationTimes = new DateTime[count];
            void Receiver(object obj, ElapsedEventArgs args)
            {
            };

            //Act
            ITimeMonitor timeMonitor = new TimeMonitor(notificationTimes, Receiver);

            //Assert
            Assert.Equal(count, timeMonitor.TimeKeepers.Count());
        }
    }
}
