﻿// Copyright (c) the authors of NanoGames. All rights reserved.
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
        private const double _lineRadius = 0.5;

        private static readonly VertexSpecification _vertexSpecification = new VertexSpecification()
        {
            { 0, 2, VertexAttribPointerType.Float, false },
            { 1, 2, VertexAttribPointerType.Float, false },
            { 2, 1, VertexAttribPointerType.Float, false },
            { 3, 3, VertexAttribPointerType.UnsignedByte, true },
        };

        private readonly Shader _shader = new Shader("NanoGames.Shaders.Line");
        private readonly TriangleBuffer _buffer = new TriangleBuffer(_vertexSpecification);
        private readonly VertexArray _vertexArray = new VertexArray(_vertexSpecification);

        private readonly PostProcessor _postProcessor = new PostProcessor();

        private readonly int _framebufferId;
        private readonly int _frameTextureId;

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
            _framebufferId = GL.GenFramebuffer();
            _frameTextureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, _frameTextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 0, 0, 0, 0 });

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _framebufferId);
            GL.FramebufferTexture(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, _frameTextureId, 0);

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GL.DeleteFramebuffer(_framebufferId);
            _postProcessor.Dispose();
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
            _postProcessor.BeginFrame(width, height);

            if ((double)width / (double)height > Terminal.Width / Terminal.Height)
            {
                width = (int)Math.Ceiling(height / Terminal.Height * Terminal.Width);
            }
            else
            {
                height = (int)Math.Ceiling(width / Terminal.Width * Terminal.Height);
            }

            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;
                GL.BindTexture(TextureTarget.Texture2D, _frameTextureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            }

            _buffer.Clear();

            double screenAspect = (double)_width / (double)_height;
            double terminalAspect = (double)Terminal.Width / (double)Terminal.Height;

            if (terminalAspect > screenAspect)
            {
                _xScale = 2.0 / Terminal.Width;
                _xOffset = -1;
                _yScale = 2.0 / Terminal.Height * (screenAspect / terminalAspect);
                _yOffset = -1;
            }
            else
            {
                _xScale = 2.0 / Terminal.Width * (terminalAspect / screenAspect);
                _xOffset = -1;
                _yScale = 2.0 / Terminal.Height;
                _yOffset = -1;
            }
        }

        /// <summary>
        /// Ends the frame and issues all draw commands.
        /// </summary>
        public void EndFrame()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _framebufferId);

            GL.Viewport(0, 0, _width, _height);
            GL.ClearColor(0.0625f, 0.0625f, 0.0625f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Bind();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            _vertexArray.SetData(_buffer, BufferUsageHint.StreamDraw);
            _vertexArray.Draw();
            GL.Disable(EnableCap.Blend);

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            GL.BindTexture(TextureTarget.Texture2D, _frameTextureId);
            _postProcessor.EndFrame();
        }

        /// <inheritdoc/>
        public void Line(Color color, Vector vectorA, Vector vectorB)
        {
            var r = GetColorValue(color.R);
            var g = GetColorValue(color.G);
            var b = GetColorValue(color.B);

            var v = vectorB - vectorA;
            var vectorLength = v.Length;
            v *= _lineRadius / vectorLength;
            var w = v.RotatedLeft;
            float lineLength = (float)(vectorLength / _lineRadius);

            _buffer.Triangle(0, 1, 3);
            _buffer.Triangle(1, 2, 3);

            WriteVector(vectorA - v + w);
            _buffer.VertexFloat(-1, 1);
            _buffer.VertexFloat(lineLength);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();

            WriteVector(vectorA - v - w);
            _buffer.VertexFloat(-1, -1);
            _buffer.VertexFloat(lineLength);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();

            WriteVector(vectorB + v - w);
            _buffer.VertexFloat(lineLength + 1, -1);
            _buffer.VertexFloat(lineLength);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();

            WriteVector(vectorB + v + w);
            _buffer.VertexFloat(lineLength + 1, 1);
            _buffer.VertexFloat(lineLength);
            _buffer.VertexByte(r, g, b);
            _buffer.EndVertex();
        }

        private static byte GetColorValue(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value * 255));
        }

        private void WriteVector(Vector v)
        {
            _buffer.VertexFloat((float)(v.X * _xScale + _xOffset), (float)(v.Y * _yScale + _yOffset));
        }
    }
}
