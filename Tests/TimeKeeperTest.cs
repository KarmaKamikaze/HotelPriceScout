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
            void Receiver(object obj, ElapsedEventArgs args)
            {
            };
            
            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(minutes, Receiver);
            
            //Assert
            Assert.IsType<Timer>(timeKeeper.Timer);
        }
        
        [Fact]
        public void TimeKeeperDailyTriggerConstructorCreatesTimerTest()
        {
            //Arrange
            int hourOfDay = 10;
            int minuteOfDay = 10;
            void Receiver(object obj, ElapsedEventArgs args)
            {
            };
            
            //Act
            ITimeKeeper timeKeeper = new TimeKeeper(hourOfDay, minuteOfDay, Receiver);
            
            //Assert
            Assert.IsType<Timer>(timeKeeper.Timer);
        }
    }
}