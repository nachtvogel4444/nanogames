// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine.OpenGLWrappers;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;

namespace NanoGames.Engine
{
    /// <summary>
    /// Pre-processor that implements the CRT fade effect.
    /// </summary>
    internal sealed class PreProcessor : IDisposable
    {
        private static readonly double _fadeFactor = Math.Log(0.5) * 60;

        private static readonly VertexSpecification _fadeVertexSpecification = new VertexSpecification()
        {
            { 0, 2, VertexAttribPointerType.Float, false },
        };

        private readonly Shader _shader = new Shader("NanoGames.Engine.Shaders.Fade");

        private readonly TriangleBuffer _buffer = new TriangleBuffer(_fadeVertexSpecification);

        private readonly VertexArray _vertexArray = new VertexArray(_fadeVertexSpecification);

        private long _lastTimestamp = Stopwatch.GetTimestamp();

        /// <summary>
        /// Initializes a new instance of the <see cref="PreProcessor"/> class.
        /// </summary>
        public PreProcessor()
        {
            _buffer.Clear();
            _buffer.Triangle(0, 1, 2);
            _buffer.Triangle(0, 2, 3);
            _buffer.VertexFloat(-1, -1);
            _buffer.EndVertex();
            _buffer.VertexFloat(1, -1);
            _buffer.EndVertex();
            _buffer.VertexFloat(1, 1);
            _buffer.EndVertex();
            _buffer.VertexFloat(-1, 1);
            _buffer.EndVertex();
            _vertexArray.SetData(_buffer, BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Gets the fade multiplier.
        /// </summary>
        public float Fade { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            _vertexArray.Dispose();
            _buffer.Dispose();
            _shader.Dispose();
        }

        /// <summary>
        /// Partially clears the screen.
        /// </summary>
        internal void Clear()
        {
            long currentTimestamp = Stopwatch.GetTimestamp();
            double timeDelta = (currentTimestamp - _lastTimestamp) / (double)Stopwatch.Frequency;
            _lastTimestamp = currentTimestamp;

            Fade = 1 - (float)Math.Exp(_fadeFactor * timeDelta);

            _shader.Bind();
            GL.Uniform1(0, Fade);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            _vertexArray.Draw();
            GL.Disable(EnableCap.Blend);
        }
    }
}
