using System;
using System.Globalization;
using System.Timers;

namespace HotelPriceScout.Data.Function
{
    public class TimeKeeper
    {
        public TimeKeeper(int hourOfDay, int minuteOfDay, ElapsedEventHandler receiver)
        {
            Timer = SetupTimer(SetTimeTrigger(hourOfDay, minuteOfDay), receiver);
        }

        public Timer Timer { get; }

        private TimeSpan SetTimeTrigger(int hourOfDay, int minuteOfDay)
        {
            TimeSpan day = new TimeSpan(24, 00, 00);
            TimeSpan now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture));
            TimeSpan triggerTime = new TimeSpan(hourOfDay, minuteOfDay, 0);

            return day - now + triggerTime;
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