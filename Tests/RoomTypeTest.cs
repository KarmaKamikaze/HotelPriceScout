using System;
using Xunit;

namespace Tests
{
    public class RoomTypeTest
    {
        [InlineData(1)]
        [InlineData(2)]
        [Theory]
        public void Test_Capacity_For_Validity_One(int value)
        {
            void SetCapacity(int value)
            {

            }


            Assert.Equal(1, value);
            Assert.Throws<ArgumentOutOfRangeException>(() => SetCapacity(value));
        }



    }
}