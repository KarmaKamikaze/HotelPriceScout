using System;
using System.Collections.Generic;
using System.Timers;

namespace HotelPriceScout.Data.Function
{
    public class TimeMonitor
    {
        public TimeMonitor(IEnumerable<DateTime> notificationTimes, ElapsedEventHandler eventReceiver)
        {
            TimeKeepers = SetUpNotificationTimers(notificationTimes, eventReceiver);
        }
        
        public IEnumerable<TimeKeeper> TimeKeepers { get; }

        private IEnumerable<TimeKeeper> SetUpNotificationTimers(IEnumerable<DateTime> notificationTimes, ElapsedEventHandler eventReceiver)
        {
            List<TimeKeeper> result = new List<TimeKeeper>();
            foreach (DateTime notificationTime in notificationTimes)
            {
                result.Add(new TimeKeeper(notificationTime.Hour, notificationTime.Minute, eventReceiver));
            }

            return result;
        }
    }
}