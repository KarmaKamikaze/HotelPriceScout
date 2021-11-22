﻿using System;
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
        [InlineData(-1000, null)]
        [InlineData(-1, null)]
        [InlineData(101, null)]
        [InlineData(1000, null)]
        public async Task Check_If_MarginValue_Throws(int marginvalue, DateTime[] value)
        {
            //Arrange and act
            async Task ActionAsync()
            {
                Scout scout = await Scout.CreateScoutAsync(marginvalue, value);
            }
            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(ActionAsync);
        }

        [Theory]
        [InlineData(0, null)]
        [InlineData(55, null)]
        [InlineData(100, null)]
        public async Task Check_If_State_Throws(int marginvalue, DateTime[] value)
        {
            async Task ActionAsync()
            {
                Scout scout = await Scout.CreateScoutAsync(marginvalue, value);
            }
            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(ActionAsync);
        }
    }
}
