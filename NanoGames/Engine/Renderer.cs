// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine.OpenGLWrappers;
using OpenTK.Graphics.OpenGL4;
using System;

namespace NanoGames.Engine
{
    /// <summary>
    /// The 2D renderer implementation.
    /// </summary>
    internal sealed class Renderer : IRenderer, IDisposable
    {
        private const double _lineWidth = 1;

        private static readonly VertexSpecification _vertexSpecification = new VertexSpecification()
        {
            { 0, 2, VertexAttribPointerType.Short, true },
            { 1, 3, VertexAttribPointerType.UnsignedByte, true },
        };

        private readonly Shader _shader = new Shader("NanoGames.Shaders.Line");
        private readonly TriangleBuffer _buffer = new TriangleBuffer(_vertexSpecification);
        private readonly VertexArray _vertexArray = new VertexArray(_vertexSpecification);

        private int _width;
        private int _height;

        private double _xScale;
        private double _xOffset;
        private double _yScale;
        private double _yOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        public Renderer()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _vertexArray.Dispose();
            _buffer.Dispose();
            _shader.Dispose();
        }

        /// <summary>
        /// Begins a new frame and sets the dimensions of the render area.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        public void BeginFrame(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _width = width;
            _height = height;
            _buffer.Clear();

            double screenAspect = (double)_width / (double)_height;
            double terminalAspect = (double)Terminal.Width / (double)Terminal.Height;

            if (terminalAspect > screenAspect)
            {
                _xScale = 2 * short.MaxValue / Terminal.Width;
                _xOffset = -short.MaxValue;
                _yScale = 2 * short.MaxValue / Terminal.Height * (screenAspect / terminalAspect);
                _yOffset = -short.MinValue;
            }
            else
            {
                _xScale = 2 * short.MaxValue / Terminal.Width * (terminalAspect / screenAspect);
                _xOffset = -short.MaxValue;
                _yScale = 2 * short.MaxValue / Terminal.Height;
                _yOffset = -short.MinValue;
            }
        }

        /// <summary>
        /// Ends the frame and issues all draw commands.
        /// </summary>
        public void EndFrame()
        {
            _shader.Bind();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            _vertexArray.SetData(_buffer, BufferUsageHint.StreamDraw);
            _vertexArray.Draw();
            GL.Disable(EnableCap.Blend);
        }

        /// <inheritdoc/>
        public void Line(Color color, Vector vectorA, Vector vectorB)
        {
            var r = GetColorValue(color.R);
            var g = GetColorValue(color.G);
            var b = GetColorValue(color.B);

            var lineOffset = (vectorB - vectorA).Normalized.RotatedLeft * (0.5 * _lineWidth);

            _buffer.Triangle(0, 1, 3);
            _buffer.Triangle(1, 2, 3);

            WriteVector(vectorA + lineOffset);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();

            WriteVector(vectorA - lineOffset);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();

            WriteVector(vectorB - lineOffset);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();

            WriteVector(vectorB + lineOffset);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();
        }

        private static byte GetColorValue(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value * 255));
        }

        private void WriteVector(Vector v)
        {
            _buffer.VertexInt16((short)(v.X * _xScale + _xOffset), (short)(v.Y * _yScale + _yOffset));
        }
    }
}
