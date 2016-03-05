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
        private const double _lineRadius = 0.5;

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

            _lineBuffer.Clear();
            _pointBuffer.Clear();

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

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

            _lineShader.Bind();
            _lineVertexArray.SetData(_lineBuffer, BufferUsageHint.StreamDraw);
            _lineVertexArray.Draw();
            _lineVertexArray.Dispose();

            _pointShader.Bind();
            _pointVertexArray.SetData(_pointBuffer, BufferUsageHint.StreamDraw);
            _pointVertexArray.Draw();
            _pointVertexArray.Dispose();

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

            _lineBuffer.Triangle(0, 1, 3);
            _lineBuffer.Triangle(1, 2, 3);

            WriteVector(_lineBuffer, vectorA - v + w);
            _lineBuffer.VertexFloat(-1, 1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();

            WriteVector(_lineBuffer, vectorA - v - w);
            _lineBuffer.VertexFloat(-1, -1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();

            WriteVector(_lineBuffer, vectorB + v - w);
            _lineBuffer.VertexFloat(lineLength + 1, -1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();

            WriteVector(_lineBuffer, vectorB + v + w);
            _lineBuffer.VertexFloat(lineLength + 1, 1);
            _lineBuffer.VertexFloat(lineLength);
            _lineBuffer.VertexByte(r, g, b);
            _lineBuffer.EndVertex();
        }

        /// <inheritdoc/>
        public void Point(Color color, Vector vector)
        {
            ////double fo = 1;
            ////Line(color, vector + new Vector(-fo, -fo), vector + new Vector(fo, -fo));
            ////Line(color, vector + new Vector(fo, -fo), vector + new Vector(fo, fo));
            ////Line(color, vector + new Vector(fo, fo), vector + new Vector(-fo, fo));
            ////Line(color, vector + new Vector(-fo, fo), vector + new Vector(-fo, -fo));
            ////return;

            var r = GetColorValue(color.R);
            var g = GetColorValue(color.G);
            var b = GetColorValue(color.B);

            _pointBuffer.Triangle(0, 1, 2);
            _pointBuffer.Triangle(0, 2, 3);

            WriteVector(_pointBuffer, vector - new Vector(-_lineRadius, -_lineRadius));
            _pointBuffer.VertexFloat(-1, -1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();

            WriteVector(_pointBuffer, vector - new Vector(_lineRadius, -_lineRadius));
            _pointBuffer.VertexFloat(1, -1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();

            WriteVector(_pointBuffer, vector - new Vector(_lineRadius, _lineRadius));
            _pointBuffer.VertexFloat(1, 1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();

            WriteVector(_pointBuffer, vector - new Vector(-_lineRadius, _lineRadius));
            _pointBuffer.VertexFloat(-1, 1);
            _pointBuffer.VertexByte(r, g, b);
            _pointBuffer.EndVertex();
        }

        private static byte GetColorValue(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value * 255));
        }

        private void WriteVector(TriangleBuffer buffer, Vector v)
        {
            buffer.VertexFloat((float)(v.X * _xScale + _xOffset), (float)(v.Y * _yScale + _yOffset));
        }
    }
}
