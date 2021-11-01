using System;

namespace HotelPriceScout.Data.Model
{
  public class RoomTypePrice
  {

    private DateTime _date;
    private decimal _price;

    public RoomTypePrice(DateTime date, decimal price)
    {
      Date = date;
      Price = price;
    }

    public DateTime Date
    {
      get => _date;
      set
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
          throw new ArgumentOutOfRangeException();
        }
        _price = value;
      }
    }
  }
}