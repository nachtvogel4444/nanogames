// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Runtime.InteropServices;

namespace NanoGames.Engine.OpenGLWrappers
{
    /// <summary>
    /// A buffer storing bytes in unmanaged memory.
    /// </summary>
    internal sealed class ByteBuffer : IDisposable
    {
        private const int _initialCapacity = 1024;

        private IntPtr _data;
        private int _capacity;
        private int _byteCount;

        /// <summary>
        /// Finalizes an instance of the <see cref="ByteBuffer"/> class.
        /// </summary>
        ~ByteBuffer()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the number of bytes stored in the buffer.
        /// </summary>
        public int ByteCount => _byteCount;

        /// <summary>
        /// Gets a pointer to the first byte in the buffer.
        /// </summary>
        public IntPtr Ptr => _data;

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear()
        {
            _byteCount = 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_data != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_data);
                _data = IntPtr.Zero;
            }

            _byteCount = 0;
            _capacity = 0;
        }

        /// <summary>
        /// Allocates a number of 1-byte elements.
        /// </summary>
        /// <param name="count">The number of elements to allocate.</param>
        /// <returns>A pointer to the first element allocated.</returns>
        public IntPtr Allocate1(int count)
        {
            EnsureCapacity(_byteCount + count);
            var data = _data + _byteCount;
            _byteCount += count;
            return data;
        }

        /// <summary>
        /// Allocates a number of 2-byte elements.
        /// </summary>
        /// <param name="count">The number of elements to allocate.</param>
        /// <returns>A pointer to the first element allocated.</returns>
        public IntPtr Allocate2(int count)
        {
            int size = count * 2;
            var alignedByteCount = (_byteCount + 1) & ~1;
            EnsureCapacity(alignedByteCount + size);
            var data = _data + alignedByteCount;
            _byteCount = alignedByteCount + size;
            return data;
        }

        /// <summary>
        /// Allocates a number of 4-byte elements.
        /// </summary>
        /// <param name="count">The number of elements to allocate.</param>
        /// <returns>A pointer to the first element allocated.</returns>
        public IntPtr Allocate4(int count)
        {
            int size = count * 4;
            var alignedByteCount = (_byteCount + 3) & ~3;
            EnsureCapacity(alignedByteCount + size);
            var data = _data + alignedByteCount;
            _byteCount = alignedByteCount + size;
            return data;
        }

        /// <summary>
        /// Enlarges this byte buffer until it is aligned to the specified value.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        public void Align(int alignment)
        {
            var alignedByteCount = ((_byteCount + (alignment - 1)) / alignment) * alignment;
            EnsureCapacity(alignedByteCount);
            _byteCount = alignedByteCount;
        }

        private void EnsureCapacity(int requiredCapacity)
        {
            if (_capacity >= requiredCapacity)
            {
                return;
            }

            /* Grow at least by a factor of 2 to avoid too many reallocations. */
            _capacity = Math.Max(requiredCapacity, Math.Max(_capacity * 2, _initialCapacity));

            /* Nonsensically, ReAllocHGlobal throws an InsufficientMemoryException if passed IntPtr.Zero as the previous buffer. */
            _data = _data == IntPtr.Zero ? Marshal.AllocHGlobal(_capacity) : Marshal.ReAllocHGlobal(_data, (IntPtr)_capacity);
        }
    }
}
