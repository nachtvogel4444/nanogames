// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Helper class to create a deep clone of an entire object graph including its relationships.
    /// This is used by the netcode to create "snapshots" of the game states that can be rolled back to in order to synchronize the clients.
    /// </summary>
    internal static class Cloning
    {
        /*
         * Clones objects by copying all fields via reflection while keeping a dictionary of already cloned objects to preserve relationships and cycles.
         *
         * The reflection metadata is partially cached, but other than that, not much performance tuning has been done.
         * In particular, it's likely that dynamic code generation is a significant win for cloning public fields.
         *
         * There are unit tests for many relevant special cases in NanoGames.Tests.CloningTests.
         */

        private static readonly ConcurrentDictionary<Type, object> _cloners = new ConcurrentDictionary<Type, object>();

        private static readonly MethodInfo _internalCloneMethodDefinition =
            typeof(Cloning).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(m => m.Name == nameof(InternalClone) && m.IsGenericMethodDefinition)
            .Single();

        private static readonly MethodInfo _cloneArray1MethodDefinition =
            typeof(Cloning).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(m => m.Name == nameof(CloneArray1) && m.IsGenericMethodDefinition)
            .Single();

        private static readonly MethodInfo _cloneArray2MethodDefinition =
            typeof(Cloning).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(m => m.Name == nameof(CloneArray2) && m.IsGenericMethodDefinition)
            .Single();

        private static readonly ConcurrentDictionary<Type, MethodInfo> _internalCloneMethods = new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        /// Recursively clones the specified object and all objects referenced by it.
        /// Supports arbitrary object graphs, including recursive references.
        /// </summary>
        /// <typeparam name="T">The type to copy.</typeparam>
        /// <param name="value">The value to copy.</param>
        /// <returns>The copied value.</returns>
        public static T Clone<T>(T value)
        {
            return InternalClone(new Dictionary<object, object>(ReferenceComparer<object>.Instance), value);
        }

        private static T InternalClone<T>(Dictionary<object, object> clones, T value)
        {
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                return value;
            }
            else if (typeof(T).IsValueType)
            {
                var cloner = (StructCloner<T>)_cloners.GetOrAdd(typeof(T), StructCloner<T>.Create);
                return cloner.Clone(clones, value);
            }
            else if (typeof(T).IsArray)
            {
                var rank = typeof(T).GetArrayRank();
                if (rank == 1)
                {
                    return (T)_cloneArray1MethodDefinition
                        .MakeGenericMethod(typeof(T).GetElementType())
                        .Invoke(null, new object[] { clones, value });
                }
                else if (rank == 2)
                {
                    return (T)_cloneArray2MethodDefinition
                        .MakeGenericMethod(typeof(T).GetElementType())
                        .Invoke(null, new object[] { clones, value });
                }
                else
                {
                    throw new NotImplementedException($"Cloning {rank}-dimensional arrays is not implemented yet.");
                }
            }
            else if (ReferenceEquals(null, value))
            {
                return default(T);
            }
            else if (typeof(T).IsSealed || value.GetType() == typeof(T))
            {
                var cloner = (ClassCloner<T>)_cloners.GetOrAdd(typeof(T), ClassCloner<T>.Create);
                return cloner.Clone(clones, value);
            }
            else
            {
                var internalCloneMethod = _internalCloneMethods.GetOrAdd(value.GetType(), CreateInternalCloneMethod);
                return (T)internalCloneMethod.Invoke(null, new object[] { clones, value });
            }
        }

        private static MethodInfo CreateInternalCloneMethod(Type t)
        {
            return _internalCloneMethodDefinition.MakeGenericMethod(t);
        }

        private static IEnumerable<FieldInfo> GetFields(Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Concat(GetFields(type.BaseType));
        }

        private static T[] CloneArray1<T>(Dictionary<object, object> clones, T[] value)
        {
            long length = value.LongLength;

            var clone = new T[length];
            clones[value] = clone;

            for (long i = 0; i < length; ++i)
            {
                clone[i] = InternalClone(clones, value[i]);
            }

            return clone;
        }

        private static T[,] CloneArray2<T>(Dictionary<object, object> clones, T[,] value)
        {
            long length0 = value.GetLongLength(0);
            long length1 = value.GetLongLength(1);

            var clone = new T[length0, length1];
            clones[value] = clone;

            for (long i0 = 0; i0 < length0; ++i0)
            {
                for (long i1 = 0; i1 < length1; ++i1)
                {
                    clone[i0, i1] = InternalClone(clones, value[i0, i1]);
                }
            }

            return clone;
        }

        private sealed class StructCloner<T>
        {
            private readonly FieldInfo[] _fields;

            private StructCloner()
            {
                _fields = GetFields(typeof(T)).ToArray();
            }

            public static StructCloner<T> Create(Type unused)
            {
                return new StructCloner<T>();
            }

            public T Clone(Dictionary<object, object> clones, T value)
            {
                var clone = default(T);

                var valueRef = __makeref(value);
                var copyRef = __makeref(clone);

                for (int i = 0; i < _fields.Length; ++i)
                {
                    var field = _fields[i];
                    var fieldValue = InternalClone(clones, field.GetValueDirect(valueRef));
                    field.SetValueDirect(copyRef, fieldValue);
                }

                return clone;
            }
        }

        private sealed class ClassCloner<T>
        {
            private readonly FieldInfo[] _fields;

            private ClassCloner()
            {
                if (typeof(T).GetCustomAttributes<NonClonableAttribute>(true).Any())
                {
                    _fields = null;
                }
                else
                {
                    _fields = GetFields(typeof(T)).ToArray();
                }
            }

            public static ClassCloner<T> Create(Type unused)
            {
                return new ClassCloner<T>();
            }

            public T Clone(Dictionary<object, object> copies, T value)
            {
                if (_fields == null)
                {
                    return default(T);
                }

                object copy;
                if (copies.TryGetValue(value, out copy))
                {
                    return (T)copy;
                }

                /* Create an instance of T without running the default constructor. */
                copy = FormatterServices.GetUninitializedObject(typeof(T));
                copies[value] = copy;

                var data = FormatterServices.GetObjectData(value, _fields);
                for (int i = 0; i < data.Length; ++i)
                {
                    data[i] = InternalClone(copies, data[i]);
                }

                FormatterServices.PopulateObjectMembers(copy, _fields, data);
                return (T)copy;
            }
        }

        private sealed class ReferenceComparer<T> : IEqualityComparer<T>
        {
            public static readonly ReferenceComparer<T> Instance = new ReferenceComparer<T>();

            private ReferenceComparer()
            {
            }

            public bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}
