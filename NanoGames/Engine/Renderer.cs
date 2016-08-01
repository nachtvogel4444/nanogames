// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine.OpenGLWrappers;
using OpenTK.Graphics.OpenGL4;
using System;

namespace NanoGames.Engine
{
    /// <summary>
    /// The 2D renderer implementation.
    /// </summary>
    [Synchronization.NonClonable]
    internal sealed class Renderer : IGraphics, IDisposable
    {
        private const float _lineRadius = 0.5f;

        private static readonly VertexSpecification _lineVertexSpecification = new VertexSpecification()
        {
            { 0, 2, VertexAttribPointerType.Float, false },
            { 1, 2, VertexAttribPointerType.Float, false },
            { 2, 1, VertexAttribPointerType.Float, false },
            { 3, 3, VertexAttribPointerType.UnsignedByte, true },
        };

        private static readonly VertexSpecification _pointVertexSpecification = new VertexSpecification()
        {
            { 0, 2, VertexAttribPointerType.Float, false },
            { 1, 2, VertexAttribPointerType.Float, false },
            { 2, 3, VertexAttribPointerType.UnsignedByte, true },
        };

        private readonly Shader _lineShader = new Shader("NanoGames.Engine.Shaders.Line");
        private readonly TriangleBuffer _lineBuffer = new TriangleBuffer(_lineVertexSpecification);
        private readonly VertexArray _lineVertexArray = new VertexArray(_lineVertexSpecification);

        private readonly Shader _pointShader = new Shader("NanoGames.Engine.Shaders.Point");
        private readonly TriangleBuffer _pointBuffer = new TriangleBuffer(_pointVertexSpecification);
        private readonly VertexArray _pointVertexArray = new VertexArray(_pointVertexSpecification);

        private readonly PreProcessor _preProcessor = new PreProcessor();
        private readonly PostProcessor _postProcessor = new PostProcessor();

        private readonly Framebuffer _framebuffer;

        private int _width;
        private int _height;

        private float _xScale;
        private float _xOffset;
        private float _yScale;
        private float _yOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        public Renderer()
        {
            _framebuffer = new Framebuffer();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _framebuffer.Dispose();
            _postProcessor.Dispose();
            _preProcessor.Dispose();
            _pointVertexArray.Dispose();
            _pointBuffer.Dispose();
            _pointShader.Dispose();
            _lineVertexArray.Dispose();
            _lineBuffer.Dispose();
            _lineShader.Dispose();
        }

        /// <summary>
        /// Begins a new frame and sets the dimensions of the render area.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        public void BeginFrame(int width, int height)
        {
            _postProcessor.BeginFrame(width, height);

            if ((double)width / (double)height > GraphicsConstants.Width / GraphicsConstants.Height)
            {
                width = (int)Math.Ceiling(height / GraphicsConstants.Height * GraphicsConstants.Width);
            }
            else
            {
                height = (int)Math.Ceiling(width / GraphicsConstants.Width * GraphicsConstants.Height);
            }

            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;
                _framebuffer.SetSize(width, height);
            }

            _lineBuffer.Clear();
            _pointBuffer.Clear();

            float screenAspect = (float)_width / (float)_height;
            float terminalAspect = (float)GraphicsConstants.Width / (float)GraphicsConstants.Height;

            if (terminalAspect > screenAspect)
            {
                _xScale = 2f / (float)GraphicsConstants.Width;
                _xOffset = -1;
                _yScale = 2f / (float)GraphicsConstants.Height * (screenAspect / terminalAspect);
                _yOffset = -1;
            }
            else
            {
                _xScale = 2f / (float)GraphicsConstants.Width * (terminalAspect / screenAspect);
                _xOffset = -1;
                _yScale = 2f / (float)GraphicsConstants.Height;
                _yOffset = -1;
            }
        }

        /// <summary>
        /// Ends the frame and issues all draw commands.
        /// </summary>
        public void EndFrame()
        {
            _framebuffer.BindFramebuffer();

            GL.Viewport(0, 0, _width, _height);
            _preProcessor.Clear();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

            _lineShader.Bind();
            GL.Uniform1(0, _preProcessor.Fade);
            _lineVertexArray.SetData(_lineBuffer, BufferUsageHint.StreamDraw);
            _lineVertexArray.Draw();
            _lineVertexArray.Dispose();

            _pointShader.Bind();
            GL.Uniform1(0, _preProcessor.Fade);
            _pointVertexArray.SetData(_pointBuffer, BufferUsageHint.StreamDraw);
            _pointVertexArray.Draw();
            _pointVertexArray.Dispose();

            GL.Disable(EnableCap.Blend);

            Framebuffer.Unbind();

            _postProcessor.EndFrame(_framebuffer);
        }

        /// <inheritdoc/>
        public void Line(Color color, Vector begin, Vector end)
        {
            var r = GetColorValue(color.R);
            var g = GetColorValue(color.G);
            var b = GetColorValue(color.B);

            float ax = (float)begin.X, ay = 200f - (float)begin.Y, bx = (float)end.X, by = 200f - (float)end.Y;

            var vx = bx - ax;
            float vy = by - ay;
            var vectorLength = (float)Math.Sqrt(vx * vx + vy * vy);
            float vf = _lineRadius / vectorLength;
            vx *= vf;
            vy *= vf;

            float lineLength = vectorLength / _lineRadius;

            _lineBuffer.Triangle(0, 1, 3);
            _lineBuffer.Triangle(1, 2, 3);

            WriteVector(_lineBuffer, ax - vx - vy, ay - vy + vx);
            _lineBuffer.VertexFloat(-1, 1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();

            WriteVector(_lineBuffer, ax - vx + vy, ay - vy - vx);
            _lineBuffer.VertexFloat(-1, -1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();

            WriteVector(_lineBuffer, bx + vx + vy, by + vy - vx);
            _lineBuffer.VertexFloat(lineLength + 1, -1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();

            WriteVector(_lineBuffer, bx + vx - vy, by + vy + vx);
            _lineBuffer.VertexFloat(lineLength + 1, 1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();
        }

        /// <inheritdoc/>
        public void Point(Color color, Vector vector)
        {
            var r = GetColorValue(color.R);
            var g = GetColorValue(color.G);
            var b = GetColorValue(color.B);

            float x = (float)vector.X, y = 200f - (float)vector.Y;

            _pointBuffer.Triangle(0, 1, 2);
            _pointBuffer.Triangle(0, 2, 3);

            WriteVector(_pointBuffer, x - _lineRadius, y - _lineRadius);
            _pointBuffer.VertexFloat(-1, -1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();

            WriteVector(_pointBuffer, x + _lineRadius, y - _lineRadius);
            _pointBuffer.VertexFloat(1, -1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();

            WriteVector(_pointBuffer, x + _lineRadius, y + _lineRadius);
            _pointBuffer.VertexFloat(1, 1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();

            WriteVector(_pointBuffer, x - _lineRadius, y + _lineRadius);
            _pointBuffer.VertexFloat(-1, 1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();
        }

        private static byte GetColorValue(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value * 255));
        }

        private void WriteVector(TriangleBuffer buffer, float x, float y)
        {
            buffer.VertexFloat(x * _xScale + _xOffset, y * _yScale + _yOffset);
        }
    }
}
