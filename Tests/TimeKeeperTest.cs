using System;
using System.Globalization;
using System.Timers;
using HotelPriceScout.Data.Function;
using Xunit;

namespace Tests
{
    public class TimeKeeperTest
    {
        [Fact]
        public void TimeKeeperMinuteTriggerConstructorCreatesTimerTest()
        {
            //Arrange
            int minutes = 10;

            void Receiver(object obj, ElapsedEventArgs args){}

            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(minutes, Receiver);

            //Assert
            Assert.IsType<Timer>(timeKeeper.Timer);
        }

        [Theory]
        [InlineData(1, 60000)]
        [InlineData(10, 600000)]
        [InlineData(100, 6000000)]
        public void TimeKeeperMinuteTriggerConstructorCreatesTimerWithCorrectIntervalTest(int minutes, double expected)
        {
            //Arrange
            void Receiver(object obj, ElapsedEventArgs args){}

            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(minutes, Receiver);

            //Assert
            Assert.Equal(expected, timeKeeper.Timer.Interval);
        }

        [Fact]
        public void TimeKeeperDailyTriggerConstructorCreatesTimerTest()
        {
            //Arrange
            int hourOfDay = 10;
            int minuteOfDay = 10;

            void Receiver(object obj, ElapsedEventArgs args) {}

            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(hourOfDay, minuteOfDay, Receiver);

            //Assert
            Assert.IsType<Timer>(timeKeeper.Timer);
        }

        [Theory]
        [InlineData(1,0)]
        [InlineData(0,1)]
        [InlineData(1,10)]
        [InlineData(0,0)]
        [InlineData(100,100)]
        public void TimeKeeperDailyTriggerConstructorCreatesTimerWithCorrectIntervalTest(int hours, int minutes)
        {
            //Arrange
            void Receiver(object obj, ElapsedEventArgs args){}
            double expected;
            
            if (hours > 24)
            {
                expected = (-TimeSpan.Parse(DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture)) +
                           new TimeSpan(hours, minutes, 0)).TotalMilliseconds;
            }
            else
            {
                expected = (new TimeSpan(24, 00, 00) -
                                   TimeSpan.Parse(DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture)) +
                                   new TimeSpan(hours, minutes, 0)).TotalMilliseconds;
            }

            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(hours, minutes, Receiver);

            //Assert
            Assert.Equal(expected, timeKeeper.Timer.Interval);
        }
    }
}
