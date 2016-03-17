// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Synchronization;
using System.Collections.Generic;
using Xunit;

namespace NanoGames.Tests
{
    public class CloningTests
    {
        [Fact]
        public void IntIsClonable()
        {
            Assert.Equal(42, Cloning.Clone(42));
        }

        [Fact]
        public void DoubleIsClonable()
        {
            // Produce an inexact double
            double d = (1.0 / 10) * 3;
            Assert.Equal(d, Cloning.Clone(d));
        }

        [Fact]
        public void StringIsClonable()
        {
            Assert.Equal("foo", Cloning.Clone("foo"));
        }

        [Fact]
        public void CustomClassIsClonable()
        {
            var foo = new CustomClass<int>(10, 20, 30, 40);
            var bar = Cloning.Clone(foo);

            Assert.Equal(foo.Field1, bar.Field1);
            Assert.Equal(foo.Field2, bar.Field2);
            Assert.Equal(foo.Field3, bar.Field3);
            Assert.Equal(foo.Field4, bar.Field4);
        }

        [Fact]
        public void CustomSubClassIsClonable()
        {
            var foo = new CustomSubClass<int>(10, 20, 30, 40, 50);
            var bar = Cloning.Clone(foo);

            Assert.Equal(foo.Field1, bar.Field1);
            Assert.Equal(foo.Field2, bar.Field2);
            Assert.Equal(foo.Field3, bar.Field3);
            Assert.Equal(foo.Field4, bar.Field4);
            Assert.Equal(foo.Field5, bar.Field5);
        }

        [Fact]
        public void CloneIsNotTheSameObject()
        {
            var foo = new CustomClass<int>(1, 2, 3, 4);
            var bar = Cloning.Clone(foo);

            Assert.False(object.ReferenceEquals(foo, bar));
        }

        [Fact]
        public void ObjectRelationshipsInClassArePreserved()
        {
            var o = new object();
            var foo = new CustomClass<object>(o, o, o, o);
            var bar = Cloning.Clone(foo);

            Assert.True(ReferenceEquals(bar.Field1, bar.Field2));
            Assert.True(ReferenceEquals(bar.Field2, bar.Field3));
            Assert.True(ReferenceEquals(bar.Field3, bar.Field4));
        }

        [Fact]
        public void RecursiveObjectsAreClonable()
        {
            var foo = new Recursive<int>();
            foo.RecursiveValue = foo;
            foo.ExtraValue = 42;

            var bar = Cloning.Clone(foo);
            Assert.Equal(42, bar.ExtraValue);
            Assert.True(ReferenceEquals(bar, bar.RecursiveValue));
        }

        [Fact]
        public void CustomStructIsClonable()
        {
            var foo = new CustomStruct<int>(10, 20, 30, 40);
            var bar = Cloning.Clone(foo);

            Assert.Equal(foo.Field1, bar.Field1);
            Assert.Equal(foo.Field2, bar.Field2);
            Assert.Equal(foo.Field3, bar.Field3);
            Assert.Equal(foo.Field4, bar.Field4);
        }

        [Fact]
        public void ObjectRelationshipsInStructArePreserved()
        {
            var o = new object();
            var foo = new CustomStruct<object>(o, o, o, o);
            var bar = Cloning.Clone(foo);

            Assert.True(ReferenceEquals(bar.Field1, bar.Field2));
            Assert.True(ReferenceEquals(bar.Field2, bar.Field3));
            Assert.True(ReferenceEquals(bar.Field3, bar.Field4));
        }

        [Fact]
        public void CloneInsideClassIsNotTheSameObject()
        {
            var c = new CustomClass<int>(1, 2, 3, 4);
            var foo = new CustomClass<CustomClass<int>>(c, c, c, c);
            var bar = Cloning.Clone(foo);

            Assert.False(ReferenceEquals(foo.Field1, bar.Field1));
        }

        [Fact]
        public void CloneInsideStructIsNotTheSameObject()
        {
            var c = new CustomClass<int>(1, 2, 3, 4);
            var foo = new CustomStruct<CustomClass<int>>(c, c, c, c);
            var bar = Cloning.Clone(foo);

            Assert.False(ReferenceEquals(foo.Field1, bar.Field1));
        }

        [Fact]
        public void CloneAutoProperties()
        {
            var foo = new ClassWithAutoProperties<int>(10, 20);
            var bar = Cloning.Clone(foo);

            Assert.Equal(foo.Field1, bar.Field1);
            Assert.Equal(foo.Field2, bar.Field2);
        }

        [Fact]
        public void CloneArray()
        {
            var foo = new int[] { 10, 20 };
            var bar = Cloning.Clone(foo);

            Assert.Equal(foo.Length, bar.Length);
            Assert.Equal(foo[0], bar[0]);
            Assert.Equal(foo[1], bar[1]);
        }

        [Fact]
        public void Clone2DArray()
        {
            var foo = new int[,] { { 10, 20, 30 }, { 40, 50, 60 } };
            var bar = Cloning.Clone(foo);

            Assert.Equal(foo.Length, bar.Length);
            Assert.Equal(foo.GetLength(0), foo.GetLength(0));
            Assert.Equal(foo.GetLength(1), foo.GetLength(1));
            for (int x = 0; x < foo.GetLength(0); ++x)
            {
                for (int y = 0; y < foo.GetLength(1); ++y)
                {
                    Assert.Equal(foo[x, y], bar[x, y]);
                }
            }
        }

        [Fact]
        public void NullIsClonable()
        {
            var foo = (CustomClass<string>)null;
            var bar = Cloning.Clone(foo);
            Assert.True(ReferenceEquals(bar, null));
        }

        [Fact]
        public void NonClonableWillNotBeCloned()
        {
            var foo = new CustomClass<NonClonableClass>(new NonClonableClass(), null, null, null);
            var bar = Cloning.Clone(foo);

            Assert.Equal(null, bar.Field1);
        }

        private struct CustomStruct<T>
        {
            public readonly T _field1;

            public T _field2;

            private readonly T _field3;

            private T _field4;

            public CustomStruct(T f1, T f2, T f3, T f4)
            {
                _field1 = f1;
                _field2 = f2;
                _field3 = f3;
                _field4 = f4;
            }

            public T Field1 => _field1;

            public T Field2 => _field2;

            public T Field3 => _field3;

            public T Field4 => _field4;

            public override bool Equals(object obj)
            {
                if (!(obj is CustomStruct<T>))
                {
                    return false;
                }

                var s = (CustomStruct<T>)obj;
                var comparer = EqualityComparer<T>.Default;

                return comparer.Equals(_field1, s._field1)
                    && comparer.Equals(_field2, s._field2)
                    && comparer.Equals(_field3, s._field3)
                    && comparer.Equals(_field4, s._field4);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class Recursive<T>
        {
            public Recursive<T> RecursiveValue;

            public T ExtraValue;
        }

        [NonClonable]
        private class NonClonableClass
        {
        }

        private class CustomSubClass<T> : CustomClass<T>
        {
            public T Field5;

            public CustomSubClass(T f1, T f2, T f3, T f4, T f5)
                : base(f1, f2, f3, f4)
            {
                Field5 = f5;
            }
        }

        private class CustomClass<T>
        {
            public readonly T _field1;

            public T _field2;

            private readonly T _field3;

            private T _field4;

            public CustomClass(T f1, T f2, T f3, T f4)
            {
                _field1 = f1;
                _field2 = f2;
                _field3 = f3;
                _field4 = f4;
            }

            public T Field1 => _field1;

            public T Field2 => _field2;

            public T Field3 => _field3;

            public T Field4 => _field4;

            public override bool Equals(object obj)
            {
                if (!(obj is CustomClass<T>))
                {
                    return false;
                }

                var s = (CustomClass<T>)obj;
                var comparer = EqualityComparer<T>.Default;

                return comparer.Equals(_field1, s._field1)
                    && comparer.Equals(_field2, s._field2)
                    && comparer.Equals(_field3, s._field3)
                    && comparer.Equals(_field4, s._field4);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        private class ClassWithAutoProperties<T>
        {
            public ClassWithAutoProperties(T field1, T field2)
            {
                Field1 = field1;
                Field2 = field2;
            }

            public T Field1 { get; set; }

            public T Field2 { get; private set; }
        }
    }
}
