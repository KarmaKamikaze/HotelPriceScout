using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Xunit;
using Assert = Xunit.Assert;


namespace Tests
{
    public class Test
    {
        [InlineData(1)]
        [InlineData(2)]
        [Theory]
        public void Test_Capacity_For_Validity_One(int value)
        {
            //var sut = new Capacity();
            //var SetCap = sut.SetCapacity(value);
            void SetCapacity(int value)
            {

            }


            Assert.Equal(1, value);
            Assert.Throws<ArgumentOutOfRangeException>(() => SetCapacity(value));
        }

        [InlineData(1)]
        [InlineData(2)]
        [Theory]
        public void Test_Capacity_For_Validity_Two(int value)
        {
            Assert.Equal(2, value);
        }

        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [Theory]
        public void Test_Capacity_For_Validity_Four(int value)
        {
            Assert.Equal(4, value);
        }



    }
    public class Capacity {
        private int _capacity;

        public int GetCapacity()
        {
            return _capacity;
        }

        public void SetCapacity(int value)
        {
            if (value != 1 && value != 2 && value != 4)
            {
                throw new ArgumentOutOfRangeException($"{nameof(value)} has to be 1, 2 or 4");
            }
            _capacity = value;
        }
    }
}


