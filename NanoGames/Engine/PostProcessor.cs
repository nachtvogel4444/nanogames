// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine.OpenGLWrappers;
using OpenTK.Graphics.OpenGL4;
using System;

namespace NanoGames.Engine
{
    /// <summary>
    /// Post-processor that renders the CRT look.
    /// </summary>
    internal sealed class PostProcessor : IDisposable
    {
        private const double _blurRadius = 64;

        private static readonly VertexSpecification _vertexSpecification = new VertexSpecification()
        {
            { 0, 2, VertexAttribPointerType.Float, false },
            { 1, 2, VertexAttribPointerType.Float, false },
        };

        private static readonly Vector[] _blurVectors = new Vector[]
        {
            new Vector(Math.Cos(30 / 180.0 * Math.PI), Math.Sin(30 / 180.0 * Math.PI)),
            new Vector(Math.Cos(120 / 180.0 * Math.PI), Math.Sin(120 / 180.0 * Math.PI)),
        };

        private readonly Shader _tubeShader = new Shader("NanoGames.Engine.Shaders.Tube");
        private readonly Shader _blurShader = new Shader("NanoGames.Engine.Shaders.Blur");

        private readonly TriangleBuffer _triangleBuffer = new TriangleBuffer(_vertexSpecification);
        private readonly VertexArray _vertexArray = new VertexArray(_vertexSpecification);

        private readonly Framebuffer[] _blurFramebuffers;

        private int _width;
        private int _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostProcessor"/> class.
        /// </summary>
        public PostProcessor()
        {
            _blurFramebuffers = new Framebuffer[_blurVectors.Length];
            for (int i = 0; i < _blurFramebuffers.Length; ++i)
            {
                _blurFramebuffers[i] = new Framebuffer();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            for (int i = 0; i < _blurFramebuffers.Length; ++i)
            {
                _blurFramebuffers[i].Dispose();
            }

            _vertexArray.Dispose();
            _triangleBuffer.Dispose();
            _blurShader.Dispose();
            _tubeShader.Dispose();
        }

        /// <summary>
        /// Begins a new frame and sets the dimensions of the render area.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        internal void BeginFrame(int width, int height)
        {
            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;

                for (int i = 0; i < _blurFramebuffers.Length; ++i)
                {
                    _blurFramebuffers[i].SetSize(width * 2 / 3, height * 2 / 3);
                }

                float w, h;
                float a = (float)(width / GraphicsConstants.Width * GraphicsConstants.Height / height);
                if (a > 1)
                {
                    h = 1;
                    w = a;
                }
                else
                {
                    w = 1;
                    h = 1 / a;
                }

                _triangleBuffer.Clear();
                _triangleBuffer.Triangle(0, 1, 2);
                _triangleBuffer.Triangle(0, 2, 3);
                _triangleBuffer.VertexFloat(-1, -1);
                _triangleBuffer.VertexFloat(-w, -h);
                _triangleBuffer.EndVertex();
                _triangleBuffer.VertexFloat(1, -1);
                _triangleBuffer.VertexFloat(w, -h);
                _triangleBuffer.EndVertex();
                _triangleBuffer.VertexFloat(1, 1);
                _triangleBuffer.VertexFloat(w, h);
                _triangleBuffer.EndVertex();
                _triangleBuffer.VertexFloat(-1, 1);
                _triangleBuffer.VertexFloat(-w, h);
                _triangleBuffer.EndVertex();
                _vertexArray.SetData(_triangleBuffer, BufferUsageHint.StaticDraw);
            }
        }

        /// <summary>
        /// Ends the frame and issues all draw commands.
        /// </summary>
        /// <param name="screenFramebuffer">The screen framebuffer.</param>
        internal void EndFrame(Framebuffer screenFramebuffer)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            screenFramebuffer.BindTexture();

            for (int i = 0; i < _blurFramebuffers.Length; ++i)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 0.02f, 0.02f, 0.02f, 1 });

                GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                var framebuffer = _blurFramebuffers[i];
                GL.Viewport(0, 0, framebuffer.Width, framebuffer.Height);
                framebuffer.BindFramebuffer();
                _blurShader.Bind();
                GL.Uniform1(0, 0);
                GL.Uniform2(1, (float)(_blurRadius * _blurVectors[i].X / _width), (float)(_blurRadius * _blurVectors[i].Y / _height));
                _vertexArray.Draw();
                framebuffer.BindTexture();
            }

            Framebuffer.Unbind();

            GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.ActiveTexture(TextureUnit.Texture1);
            screenFramebuffer.BindTexture();

            GL.Viewport(0, 0, _width, _height);

            _tubeShader.Bind();
            GL.Uniform1(0, 0);
            GL.Uniform1(1, 1);
            _vertexArray.Draw();

            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
