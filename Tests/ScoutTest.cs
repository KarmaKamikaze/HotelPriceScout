using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using HotelPriceScout.Data.Model;

namespace Tests
{
    public class ScoutTest
    {
        [Theory]
        [InlineData("stopped", -1000, null)]
        [InlineData("stopped", -1, null)]
        [InlineData("stopped", 101, null)]
        [InlineData("stopped", 1000, null)]
        public async Task Check_If_MarginValue_Throws(string state, int marginvalue, DateTime[] value)
        {
            //Arrange and act
            async Task ActionAsync()
            {
                Scout scout = await Scout.CreateScoutAsync(state, marginvalue, value);
            }
            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(ActionAsync);
        }

        [Theory]
        [InlineData("test", 0, null)]
        [InlineData("false", 55, null)]
        [InlineData("programming", 100, null)]
        public async Task Check_If_State_Throws(string state, int marginvalue, DateTime[] value)
        {
            async Task ActionAsync()
            {
                Scout scout = await Scout.CreateScoutAsync(state, marginvalue, value);
            }
            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(ActionAsync);
        }

        [Fact]
        public void Check_If_Stop_Scout_Dispose_Returns_Null()
        {
            Scout scout = new Scout();

            scout.Dispose();

            Assert.Null(scout);
        }
    }
}
