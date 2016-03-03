// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace NanoGames.Engine.OpenGLWrappers
{
    /// <summary>
    /// Describes the layout of a vertex array and how the values are bound to shader variables.
    /// </summary>
    internal sealed class VertexSpecification : System.Collections.IEnumerable
    {
        private readonly List<Entry> _entries;

        private int _vertexSize;
        private int _vertexStride;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexSpecification"/> class.
        /// </summary>
        public VertexSpecification()
        {
            _entries = new List<Entry>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexSpecification"/> class
        /// that starts as a copy of an existing vertex specification.
        /// </summary>
        /// <param name="specification">The vertex specification to copy.</param>
        public VertexSpecification(VertexSpecification specification)
        {
            _entries = new List<Entry>(specification._entries);
        }

        /// <summary>
        /// Gets the number of entries in the vertex specification.
        /// </summary>
        public int EntryCount => _entries.Count;

        /// <summary>
        /// Gets a vertex entry.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The vertex entry.</returns>
        public Entry this[int index] => _entries[index];

        /// <summary>
        /// Adds an entry to the vertex specification.
        /// </summary>
        /// <param name="location">The shader input location.</param>
        /// <param name="count">The number of elements of the entry.</param>
        /// <param name="type">The type of the elements.</param>
        /// <param name="isNormalized">Whether this entry undergoes OpenGL integer normalization.</param>
        public void Add(int location, int count, VertexAttribPointerType type, bool isNormalized)
        {
            if (count < 1 || count > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            int entryAlignment = GetAlignment(type);
            int entryOffset = Align(_vertexSize, entryAlignment);
            _entries.Add(new Entry(location, count, type, isNormalized, entryAlignment, entryOffset));

            int singleSize = GetSize(type);
            int entrySize = (count - 1) * Align(singleSize, entryAlignment) + singleSize;
            _vertexSize = entryOffset + entrySize;
            _vertexStride = Align(_vertexSize, _entries[0].Alignment);
        }

        /// <summary>
        /// Binds this vertex specification to the current shader program.
        /// </summary>
        public void Bind()
        {
            for (var i = 0; i < _entries.Count; ++i)
            {
                var entry = _entries[i];
                GL.EnableVertexAttribArray(entry.Location);
                GL.VertexAttribPointer(entry.Location, entry.Count, entry.Type, entry.IsNormalized, _vertexStride, entry.Offset);
            }
        }

        /// <inheritdoc/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private static int Align(int index, int alignment)
        {
            return (index + alignment - 1) & ~(alignment - 1);
        }

        private static int GetAlignment(VertexAttribPointerType type)
        {
            switch (type)
            {
                case VertexAttribPointerType.Byte: return 1;
                case VertexAttribPointerType.Float: return 4;
                case VertexAttribPointerType.Int: return 4;
                case VertexAttribPointerType.Short: return 2;
                case VertexAttribPointerType.UnsignedByte: return 1;
                case VertexAttribPointerType.UnsignedInt: return 4;
                case VertexAttribPointerType.UnsignedShort: return 2;
                default: throw new NotImplementedException($"VertexSpecification.GetAlignment not implemented for 'VertexAttribPointerType.{type}'");
            }
        }

        private static int GetSize(VertexAttribPointerType type)
        {
            switch (type)
            {
                case VertexAttribPointerType.Byte: return 1;
                case VertexAttribPointerType.Float: return 4;
                case VertexAttribPointerType.Int: return 4;
                case VertexAttribPointerType.Short: return 2;
                case VertexAttribPointerType.UnsignedByte: return 1;
                case VertexAttribPointerType.UnsignedInt: return 4;
                case VertexAttribPointerType.UnsignedShort: return 2;
                default: throw new NotImplementedException($"VertexSpecification.GetSize not implemented for 'VertexAttribPointerType.{type}'");
            }
        }

        /// <summary>
        /// Represents a single entry of the vertex.
        /// </summary>
        public sealed class Entry
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Entry"/> class.
            /// </summary>
            /// <param name="location">The shader input location.</param>
            /// <param name="count">The number of elements.</param>
            /// <param name="type">The element type.</param>
            /// <param name="isNormalized">A value indicating whether the entry undergoes OpenGL normalization.</param>
            /// <param name="alignment">The entry's byte alignment.</param>
            /// <param name="offset">The entry's byte offset.</param>
            public Entry(int location, int count, VertexAttribPointerType type, bool isNormalized, int alignment, int offset)
            {
                Location = location;
                Count = count;
                Type = type;
                IsNormalized = isNormalized;
                Alignment = alignment;
                Offset = offset;
            }

            /// <summary>
            /// Gets the shader input location.
            /// </summary>
            public int Location { get; }

            /// <summary>
            /// Gets the number of elements.
            /// </summary>
            public int Count { get; }

            /// <summary>
            /// Gets the element type.
            /// </summary>
            public VertexAttribPointerType Type { get; }

            /// <summary>
            /// Gets a value indicating whether the entry undergoes OpenGL integer normalization.
            /// </summary>
            public bool IsNormalized { get; }

            /// <summary>
            /// Gets the entry's byte alignment.
            /// </summary>
            public int Alignment { get; }

            /// <summary>
            /// Gets the entry's byte offset.
            /// </summary>
            public int Offset { get; }
        }
    }
}
