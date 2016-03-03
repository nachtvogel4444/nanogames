// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;
using System.Diagnostics;

namespace NanoGames.Application
{
    /// <summary>
    /// Represents a player id.
    /// </summary>
    [ProtoContract]
    public struct PlayerId
    {
        [ProtoMember(1)]
        private long _id;

        private PlayerId(long id)
        {
            _id = id;
        }

        public static bool operator ==(PlayerId a, PlayerId b)
        {
            return a._id == b._id;
        }

        public static bool operator !=(PlayerId a, PlayerId b)
        {
            return a._id != b._id;
        }

        /// <summary>
        /// Creates a new unique player id.
        /// </summary>
        /// <returns>The new player id.</returns>
        public static PlayerId Create()
        {
            return new PlayerId(Stopwatch.GetTimestamp());
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is PlayerId && (PlayerId)obj == this;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
