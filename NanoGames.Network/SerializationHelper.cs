// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;
using System.Collections.Generic;
using System.IO;

namespace NanoGames.Network
{
    /// <summary>
    /// Helper class for serialization.
    /// </summary>
    internal static class SerializationHelper
    {
        /// <summary>
        /// Serializes a value.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The serialized value.</returns>
        public static byte[] Serialize<T>(T value)
        {
            if (EqualityComparer<T>.Default.Equals(default(T), value))
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a value.
        /// </summary>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <param name="bytes">The serialized bytes.</param>
        /// <returns>The deserialized value.</returns>
        public static T Deserialize<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }
    }
}
