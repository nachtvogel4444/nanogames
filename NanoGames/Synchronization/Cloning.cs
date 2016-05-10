// Copyright (c) the authors of nanoGames. All rights reserved.
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
    public static class Cloning
    {
        /*
         * Clones objects by copying all fields via reflection while keeping a dictionary of already cloned objects to preserve relationships and cycles.
         *
         * The reflection metadata is partially cached, but other than that, not much performance tuning has been done.
         * In particular, it's likely that dynamic code generation is a significant win for cloning public fields.
         *
         * There are unit tests for many relevant special cases in NanoGames.Tests.CloningTests.
         */

        private static readonly ConcurrentDictionary<Type, Cloner> _cloners = new ConcurrentDictionary<Type, Cloner>();

        /// <summary>
        /// Recursively clones the specified object and all objects referenced by it.
        /// Supports arbitrary object graphs, including recursive references.
        /// </summary>
        /// <typeparam name="T">The type to copy.</typeparam>
        /// <param name="value">The value to copy.</param>
        /// <returns>The copied value.</returns>
        public static T Clone<T>(T value)
        {
            return GetCloner<T>().Clone(new Dictionary<object, object>(ReferenceComparer<object>.Instance), value);
        }

        private static Cloner<T> GetCloner<T>()
        {
            return (Cloner<T>)_cloners.GetOrAdd(typeof(T), CreateCloner);
        }

        private static Cloner GetCloner(Type type)
        {
            return _cloners.GetOrAdd(type, CreateCloner);
        }

        private static Cloner CreateCloner(Type t)
        {
            if (CloningIsNoop(t))
            {
                return InstantiateCloner(typeof(NoopCloner<>), t);
            }
            else if (t.IsValueType)
            {
                return InstantiateCloner(typeof(StructCloner<>), t);
            }
            else if (t.IsArray)
            {
                var rank = t.GetArrayRank();
                if (rank == 1)
                {
                    return InstantiateCloner(typeof(Array1Cloner<>), t.GetElementType());
                }
                else if (rank == 2)
                {
                    return InstantiateCloner(typeof(Array2Cloner<>), t.GetElementType());
                }
                else
                {
                    throw new NotImplementedException($"Cloning {rank}-dimensional arrays is not implemented yet.");
                }
            }
            else if (t.IsSealed)
            {
                return InstantiateCloner(typeof(ConcreteClassCloner<>), t);
            }
            else
            {
                return InstantiateCloner(typeof(BaseClassCloner<>), t);
            }
        }

        private static Cloner InstantiateCloner(Type cloner)
        {
            return (Cloner)Activator.CreateInstance(cloner);
        }

        private static Cloner InstantiateCloner(Type cloner, params Type[] typeArguments)
        {
            return (Cloner)Activator.CreateInstance(cloner.MakeGenericType(typeArguments));
        }

        private static bool CloningIsNoop(Type t)
        {
            if (t.IsPrimitive || t == typeof(string))
            {
                return true;
            }
            else if (t.IsValueType)
            {
                return GetFields(t).All(f => CloningIsNoop(f.FieldType));
            }
            else
            {
                return false;
            }
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

        private abstract class Cloner
        {
            public abstract object Clone(Dictionary<object, object> clones, object value);
        }

        private abstract class Cloner<T> : Cloner
        {
            public sealed override object Clone(Dictionary<object, object> clones, object value)
            {
                return Clone(clones, (T)value);
            }

            public abstract T Clone(Dictionary<object, object> clones, T value);
        }

        private sealed class NoopCloner<T> : Cloner<T>
        {
            public override T Clone(Dictionary<object, object> clones, T value)
            {
                return value;
            }
        }

        private sealed class Array1Cloner<T> : Cloner<T[]>
        {
            private Cloner<T> _cloner;

            public override T[] Clone(Dictionary<object, object> clones, T[] value)
            {
                if (value == null)
                {
                    return null;
                }

                if (_cloner == null)
                {
                    _cloner = GetCloner<T>();
                }

                long length = value.LongLength;

                var clone = new T[length];
                clones[value] = clone;

                for (long i = 0; i < length; ++i)
                {
                    clone[i] = _cloner.Clone(clones, value[i]);
                }

                return clone;
            }
        }

        private sealed class Array2Cloner<T> : Cloner<T[,]>
        {
            private Cloner<T> _cloner;

            public override T[,] Clone(Dictionary<object, object> clones, T[,] value)
            {
                if (value == null)
                {
                    return null;
                }

                if (_cloner == null)
                {
                    _cloner = GetCloner<T>();
                }

                long length0 = value.GetLongLength(0);
                long length1 = value.GetLongLength(1);

                var clone = new T[length0, length1];
                clones[value] = clone;

                for (long i0 = 0; i0 < length0; ++i0)
                {
                    for (long i1 = 0; i1 < length1; ++i1)
                    {
                        clone[i0, i1] = _cloner.Clone(clones, value[i0, i1]);
                    }
                }

                return clone;
            }
        }

        private sealed class StructCloner<T> : Cloner<T>
            where T : struct
        {
            private readonly FieldInfo[] _fields;
            private Cloner[] _cloners;

            public StructCloner()
            {
                _fields = GetFields(typeof(T)).ToArray();
            }

            public override T Clone(Dictionary<object, object> clones, T value)
            {
                if (_cloners == null)
                {
                    _cloners = _fields.Select(f => CreateCloner(f.FieldType)).ToArray();
                }

                var clone = default(T);

                var valueRef = __makeref(value);
                var copyRef = __makeref(clone);

                for (int i = 0; i < _fields.Length; ++i)
                {
                    var field = _fields[i];
                    var fieldValue = _cloners[i].Clone(clones, field.GetValueDirect(valueRef));
                    field.SetValueDirect(copyRef, fieldValue);
                }

                return clone;
            }
        }

        private sealed class BaseClassCloner<T> : Cloner<T>
            where T : class
        {
            private ConcreteClassCloner<T> _cloner = new ConcreteClassCloner<T>();

            public override T Clone(Dictionary<object, object> clones, T value)
            {
                if (value == null)
                {
                    return null;
                }

                var type = value.GetType();
                if (type == typeof(T))
                {
                    return _cloner.Clone(clones, value);
                }

                return (T)GetCloner(type).Clone(clones, value);
            }
        }

        private sealed class ConcreteClassCloner<T> : Cloner<T>
            where T : class
        {
            private readonly FieldInfo[] _fields;
            private Cloner[] _cloners;

            public ConcreteClassCloner()
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

            public override T Clone(Dictionary<object, object> clones, T value)
            {
                if (value == null)
                {
                    return null;
                }

                if (_fields == null)
                {
                    return default(T);
                }

                if (_cloners == null)
                {
                    _cloners = _fields.Select(f => CreateCloner(f.FieldType)).ToArray();
                }

                object copy;
                if (clones.TryGetValue(value, out copy))
                {
                    return (T)copy;
                }

                /* Create an instance of T without running the default constructor. */
                copy = FormatterServices.GetUninitializedObject(typeof(T));
                clones[value] = copy;

                var data = FormatterServices.GetObjectData(value, _fields);
                for (int i = 0; i < data.Length; ++i)
                {
                    data[i] = _cloners[i].Clone(clones, data[i]);
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
