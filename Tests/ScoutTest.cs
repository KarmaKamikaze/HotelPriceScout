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
    [InlineData("stopped", -1, null)]
    [InlineData("test", 55, null)]
    [InlineData("stopped", 101, null)]
    public async Task Check_If_MarginValue_Throws(string state, int marginvalue, DateTime[] value)
        {
            //Arrange and act
            async Task ActionAsync()
            {
                Scout scout = await Scout.CreateScoutAsync(state, marginvalue, value);
            }

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(ActionAsync);
        }


    }
}
