using System.Timers;

namespace HotelPriceScout.Data.Function
{
    public interface ITimeKeeper
    {
        Timer Timer { get; }
    }
}