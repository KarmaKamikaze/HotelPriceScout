using System.Collections.Generic;

namespace HotelPriceScout.Data.Function
{
    public interface ITimeMonitor
    {
        IEnumerable<ITimeKeeper> TimeKeepers { get; }
    }
}