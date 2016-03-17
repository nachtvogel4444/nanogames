// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using Xunit;

namespace NanoGames.Tests
{
    public class VectorTests
    {
        [Fact]
        public void ObjectEquals()
        {
            var a = new Vector(1, 2);
            var b = new Vector(1, 2);
            Assert.True(object.Equals(a, b));
        }

        [Fact]
        public void EqualsOperator()
        {
            var a = new Vector(1, 2);
            var b = new Vector(1, 2);
            Assert.True(a == b);
        }

        [Fact]
        public void GetHashCodeIsEqual()
        {
            var a = new Vector(1, 2);
            var b = new Vector(1, 2);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCodeIsUnequal()
        {
            var a = new Vector(1, 2);
            var b = new Vector(1, 3);
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ToStringIsCorrectlyFormatted()
        {
            var a = new Vector(1, 2);
            Assert.Equal("(1, 2)", a.ToString());
        }

        [Fact]
        public void TestAddition()
        {
            var a = new Vector(1, 2);
            var b = new Vector(10, 20);
            var c = a + b;
            Assert.Equal(new Vector(11, 22), c);
        }
    }
}
