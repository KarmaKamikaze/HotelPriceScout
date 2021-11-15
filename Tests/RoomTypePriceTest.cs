using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelPriceScout.Data.Model;
using Xunit;

namespace Tests
{
    public class RoomTypePriceTest
    {

        [Fact]
        public void DateSetterThrowsWhenValueIsBeforeCurrentDate()
        {
            //Arrange
            DateTime date = DateTime.Now.AddDays(-1);
            void Action()
            {
                RoomTypePrice roomTypePrice = new RoomTypePrice(date);
            }

            //Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(Action);

        }

    }
}
