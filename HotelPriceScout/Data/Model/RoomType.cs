using System;
using System.Collections.Generic;

namespace HotelPriceScout.Data.Model
{

  public class RoomType
  {
    private int _capacity;

    public int Capacity
    {
      get => _capacity;
      set
      {
        if (value is not 1 or 2 or 4)
        {
          throw new ArgumentOutOfRangeException($"{nameof(value)} has to be 1, 2 or 4");
        }
        _capacity = value;
      }
    }

    public List<RoomTypePrice> Prices { get; }

    public void Create90Prices()
    {
      DateTime currentdate = DateTime.Now;
      for (int i = 0; i < 90; i++)
      {
        Prices.Add(new RoomTypePrice(currentdate.AddDays(i)));
      }
    }

  }
}