using System;

namespace HotelPriceScout.Data.Model
{
  public class RoomTypePrice
  {

    private readonly DateTime _date;
    private decimal _price;

    public RoomTypePrice(DateTime date)
    {
      Date = date;
    }
    public RoomTypePrice(DateTime date, decimal price) : this(date)
    {
      Price = price;
    }

    public DateTime Date
    {
      get => _date;
      private init
      {
        if (value.Date < DateTime.Now.Date)
        {
          throw new ArgumentOutOfRangeException($"{nameof(value)} cannot be a date earlier than the current date.");
        }
        _date = value;
      }
    }

    public decimal Price
    {
      get => _price;
      set
      {
        if (value < 0)
        {
          throw new ArgumentOutOfRangeException($"{nameof(value)} cannot be less than zero.");
        }
        _price = value;
      }
    }
  }
}