using HotelPriceScout.Data.Model;

namespace HotelPriceScout.Data.Interface
{
    public class ScoutSharedService
    {
        public IScout Scout { get; set; }
        public bool ScoutRunning { get; set; }
    }
}
