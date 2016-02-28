// Copyright (c) the authors of NanoGames. All rights reserved.
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
        private static readonly VertexSpecification _tubeVertexSpecification = new VertexSpecification()
        {
            { 0, 2, VertexAttribPointerType.Float, false },
            { 1, 2, VertexAttribPointerType.Float, false },
        };

        private readonly Shader _shader = new Shader("NanoGames.Shaders.Tube");
        private readonly TriangleBuffer _buffer = new TriangleBuffer(_tubeVertexSpecification);
        private readonly VertexArray _vertexArray = new VertexArray(_tubeVertexSpecification);
        private int _width;
        private int _height;

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
        internal void BeginFrame(int width, int height)
        {
            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;

                float w, h;
                float a = (float)(width / Terminal.Width * Terminal.Height / height);
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

                _buffer.Clear();
                _buffer.Triangle(0, 1, 2);
                _buffer.Triangle(0, 2, 3);
                _buffer.VertexFloat(-1, -1);
                _buffer.VertexFloat(-w, -h);
                _buffer.EndVertex();
                _buffer.VertexFloat(1, -1);
                _buffer.VertexFloat(w, -h);
                _buffer.EndVertex();
                _buffer.VertexFloat(1, 1);
                _buffer.VertexFloat(w, h);
                _buffer.EndVertex();
                _buffer.VertexFloat(-1, 1);
                _buffer.VertexFloat(-w, h);
                _buffer.EndVertex();
                _vertexArray.SetData(_buffer, BufferUsageHint.StaticDraw);
            }
        }

        /// <summary>
        /// Ends the frame and issues all draw commands.
        /// </summary>
        internal void EndFrame()
        {
            GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.Viewport(0, 0, _width, _height);
            _shader.Bind();
            _vertexArray.Draw();
        }
    }
}
