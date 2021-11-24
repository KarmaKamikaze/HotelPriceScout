using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Xunit;
using HotelPriceScout.Data.Function;

namespace Tests
{

    public class TimeKeeperTest
    {
        [Fact]
        public void TimeKeeperMinuteTriggerConstructorCreatesTimerTest()
        {
            //Arrange
            int minutes = 10;

            TimeSpan triggerInterval = new TimeSpan(00, minutes, 00);
            void receiver(object obj, ElapsedEventArgs args)
            {
            };

            Timer timer = new Timer();
            timer.Interval = triggerInterval.TotalMilliseconds;
            timer.Elapsed += receiver;
            timer.AutoReset = false;
            timer.Enabled = true;
            
            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(minutes, receiver);
            
            //Assert
            Assert.Same(timer, timeKeeper.Timer);
        }
        
        [Fact]
        public void TimeKeeperDailyTriggerConstructorCreatesTimerTest()
        {
            //Arrange
            int hourOfDay = 10;
            int minuteOfDay = 10;
            
            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(hourOfDay, minuteOfDay, null);
            
            
            //Assert
            Assert.NotNull(timeKeeper.Timer);
        }
        
        

        
    }
    
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

            //Act
            TimeMonitor timeMonitor = new TimeMonitor(notificationTimes, null);

            //Assert
            Assert.Equal(count, timeMonitor.TimeKeepers.Count());
        }
        
        
        
        
        
        
        
    }
}