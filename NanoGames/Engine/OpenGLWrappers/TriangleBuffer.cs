// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;

namespace NanoGames.Engine.OpenGLWrappers
{
    /// <summary>
    /// Represents a buffer of triangles, which combines vertex and index data.
    /// </summary>
    internal sealed class TriangleBuffer : IDisposable
    {
        private readonly ByteBuffer _indices = new ByteBuffer();
        private readonly ByteBuffer _vertices = new ByteBuffer();

        private int _indexCount;
        private uint _vertexCount;

        private int _specificationIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriangleBuffer"/> class.
        /// </summary>
        /// <param name="specification">The specification for the vertices in the buffer.</param>
        public TriangleBuffer(VertexSpecification specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            Specification = specification;
        }

        /// <summary>
        /// Gets the number of triangle indices currently in the buffer.
        /// </summary>
        public int IndexCount => _indexCount;

        private VertexSpecification Specification { get; }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear()
        {
            _vertices.Clear();
            _indices.Clear();
            _indexCount = 0;
            _vertexCount = 0;
            _specificationIndex = 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _vertices.Dispose();
            _indices.Dispose();
            _indexCount = 0;
            _vertexCount = 0;
            _specificationIndex = 0;
        }

        /// <summary>
        /// Uploads the buffer contents to OpenGL.
        /// </summary>
        /// <param name="usageHint">A hint how OpenGL should use the data.</param>
        public void UploadData(BufferUsageHint usageHint)
        {
            _vertices.Align(Specification[0].Alignment);

            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)_indices.ByteCount, _indices.Ptr, usageHint);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)_vertices.ByteCount, _vertices.Ptr, usageHint);
        }

        /// <summary>
        /// Adds a new triangle. Indices are relative to the current vertex count.
        /// </summary>
        /// <param name="i0">The first vertex index.</param>
        /// <param name="i1">The second vertex index.</param>
        /// <param name="i2">The third vertex index.</param>
        public void Triangle(uint i0, uint i1, uint i2)
        {
            unsafe
            {
                var data = (uint*)_indices.Allocate4(3);
                data[0] = _vertexCount + i0;
                data[1] = _vertexCount + i1;
                data[2] = _vertexCount + i2;
            }

            _indexCount += 3;
        }

        /// <summary>
        /// Closes the vertex.
        /// </summary>
        public void EndVertex()
        {
            ++_vertexCount;
            CheckEndVertex();
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        public void VertexFloat(float v0)
        {
            CheckAttribute(1, VertexAttribPointerType.Float);
            unsafe
            {
                var data = (float*)_vertices.Allocate4(1);
                data[0] = v0;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        public void VertexFloat(float v0, float v1)
        {
            CheckAttribute(2, VertexAttribPointerType.Float);
            unsafe
            {
                var data = (float*)_vertices.Allocate4(2);
                data[0] = v0;
                data[1] = v1;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        public void VertexFloat(float v0, float v1, float v2)
        {
            CheckAttribute(3, VertexAttribPointerType.Float);
            unsafe
            {
                var data = (float*)_vertices.Allocate4(3);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        /// <param name="v3">The fourth value.</param>
        public void VertexFloat(float v0, float v1, float v2, float v3)
        {
            CheckAttribute(4, VertexAttribPointerType.Float);
            unsafe
            {
                var data = (float*)_vertices.Allocate4(4);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
                data[3] = v3;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        public void VertexByte(byte v0)
        {
            CheckAttribute(1, VertexAttribPointerType.UnsignedByte);
            unsafe
            {
                var data = (byte*)_vertices.Allocate1(1);
                data[0] = v0;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        public void VertexByte(byte v0, byte v1)
        {
            CheckAttribute(2, VertexAttribPointerType.UnsignedByte);
            unsafe
            {
                var data = (byte*)_vertices.Allocate1(2);
                data[0] = v0;
                data[1] = v1;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        public void VertexByte(byte v0, byte v1, byte v2)
        {
            CheckAttribute(3, VertexAttribPointerType.UnsignedByte);
            unsafe
            {
                var data = (byte*)_vertices.Allocate1(3);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        /// <param name="v3">The fourth value.</param>
        public void VertexByte(byte v0, byte v1, byte v2, byte v3)
        {
            CheckAttribute(4, VertexAttribPointerType.UnsignedByte);
            unsafe
            {
                var data = (byte*)_vertices.Allocate1(4);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
                data[3] = v3;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        public void VertexInt16(short v0)
        {
            CheckAttribute(1, VertexAttribPointerType.Short);
            unsafe
            {
                var data = (short*)_vertices.Allocate2(1);
                data[0] = v0;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        public void VertexInt16(short v0, short v1)
        {
            CheckAttribute(2, VertexAttribPointerType.Short);
            unsafe
            {
                var data = (short*)_vertices.Allocate2(2);
                data[0] = v0;
                data[1] = v1;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        public void VertexInt16(short v0, short v1, short v2)
        {
            CheckAttribute(3, VertexAttribPointerType.Short);
            unsafe
            {
                var data = (short*)_vertices.Allocate2(3);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        /// <param name="v3">The fourth value.</param>
        public void VertexInt16(short v0, short v1, short v2, short v3)
        {
            CheckAttribute(4, VertexAttribPointerType.Short);
            unsafe
            {
                var data = (short*)_vertices.Allocate2(4);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
                data[3] = v3;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        public void VertexUInt16(ushort v0)
        {
            CheckAttribute(1, VertexAttribPointerType.UnsignedShort);
            unsafe
            {
                var data = (ushort*)_vertices.Allocate2(1);
                data[0] = v0;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        public void VertexUInt16(ushort v0, ushort v1)
        {
            CheckAttribute(2, VertexAttribPointerType.UnsignedShort);
            unsafe
            {
                var data = (ushort*)_vertices.Allocate2(2);
                data[0] = v0;
                data[1] = v1;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        public void VertexUInt16(ushort v0, ushort v1, ushort v2)
        {
            CheckAttribute(3, VertexAttribPointerType.UnsignedShort);
            unsafe
            {
                var data = (ushort*)_vertices.Allocate2(3);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        /// <param name="v3">The fourth value.</param>
        public void VertexUInt16(ushort v0, ushort v1, ushort v2, ushort v3)
        {
            CheckAttribute(4, VertexAttribPointerType.UnsignedShort);
            unsafe
            {
                var data = (ushort*)_vertices.Allocate2(4);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
                data[3] = v3;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        public void VertexUInt32(uint v0)
        {
            CheckAttribute(1, VertexAttribPointerType.UnsignedInt);
            unsafe
            {
                var data = (uint*)_vertices.Allocate4(1);
                data[0] = v0;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        public void VertexUInt32(uint v0, uint v1)
        {
            CheckAttribute(2, VertexAttribPointerType.UnsignedInt);
            unsafe
            {
                var data = (uint*)_vertices.Allocate4(2);
                data[0] = v0;
                data[1] = v1;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        public void VertexUInt32(uint v0, uint v1, uint v2)
        {
            CheckAttribute(3, VertexAttribPointerType.UnsignedInt);
            unsafe
            {
                var data = (uint*)_vertices.Allocate4(3);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
            }
        }

        /// <summary>
        /// Writes a vertex entry.
        /// </summary>
        /// <param name="v0">The first value.</param>
        /// <param name="v1">The second value.</param>
        /// <param name="v2">The third value.</param>
        /// <param name="v3">The fourth value.</param>
        public void VertexUInt32(uint v0, uint v1, uint v2, uint v3)
        {
            CheckAttribute(4, VertexAttribPointerType.UnsignedInt);
            unsafe
            {
                var data = (uint*)_vertices.Allocate4(4);
                data[0] = v0;
                data[1] = v1;
                data[2] = v2;
                data[3] = v3;
            }
        }

        [Conditional("DEBUG")]
        private void CheckEndVertex()
        {
            if (_specificationIndex != Specification.EntryCount)
            {
                throw new InvalidOperationException("Incomplete vertex");
            }

            _specificationIndex = 0;
        }

        [Conditional("DEBUG")]
        private void CheckAttribute(int count, VertexAttribPointerType type)
        {
            var entry = Specification[_specificationIndex];
            if (entry.Count != count || entry.Type != type)
            {
                throw new InvalidOperationException("Invalid vertex attribute type");
            }

            ++_specificationIndex;
        }
    }
}
