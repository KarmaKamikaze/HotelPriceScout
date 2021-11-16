using System;
using System.Globalization;
using System.Timers;

namespace HotelPriceScout.Data.Function
{
    public class TimeKeeper
    {
        public TimeKeeper(int minutes, ElapsedEventHandler receiver)
        {
            Timer = SetupTimer(SetMinuteTrigger(minutes), receiver);
        }
        
        public TimeKeeper(int hourOfDay, int minuteOfDay, ElapsedEventHandler receiver)
        {
            Timer = SetupTimer(SetDailyTimeTrigger(hourOfDay, minuteOfDay), receiver);
        }

        public Timer Timer { get; }

        private TimeSpan SetMinuteTrigger(int minuteOfDay)
        {
            return new TimeSpan(00, minuteOfDay, 00);
        }

        private TimeSpan SetDailyTimeTrigger(int hourOfDay, int minuteOfDay)
        {
            TimeSpan day = new TimeSpan(24, 00, 00);
            TimeSpan now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture));
            TimeSpan triggerTime = new TimeSpan(hourOfDay, minuteOfDay, 0);

            TimeSpan firstTrigger = ((day - now) + triggerTime);
            if (firstTrigger.TotalHours > 24) // Make sure to trigger today, if time span is larger than 24 hours
                firstTrigger -= day;
            
            return firstTrigger;
        }

        private Timer SetupTimer(TimeSpan triggerInterval, ElapsedEventHandler receiver)
        {
            Timer timer = new Timer();
            timer.Interval = triggerInterval.TotalMilliseconds;
            timer.Elapsed += receiver;
            timer.AutoReset = true;
            timer.Enabled = true;

            return timer;
        }
    }
}