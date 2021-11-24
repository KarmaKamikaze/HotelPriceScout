using System;
using System.Collections.Generic;
using System.Timers;

namespace HotelPriceScout.Data.Function
{
    public class TimeMonitor : ITimeMonitor
    {
        public TimeMonitor(IEnumerable<DateTime> notificationTimes, ElapsedEventHandler eventReceiver)
        {
            TimeKeepers = SetUpNotificationTimers(notificationTimes, eventReceiver);
        }
        
        public IEnumerable<ITimeKeeper> TimeKeepers { get; }

        private IEnumerable<ITimeKeeper> SetUpNotificationTimers(IEnumerable<DateTime> notificationTimes, 
            ElapsedEventHandler eventReceiver)
        {
            List<ITimeKeeper> result = new List<ITimeKeeper>();
            foreach (DateTime notificationTime in notificationTimes)
            {
                result.Add(new TimeKeeper(notificationTime.Hour, notificationTime.Minute, eventReceiver));
            }

            return result;
        }
    }
}