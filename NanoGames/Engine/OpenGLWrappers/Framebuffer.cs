// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK.Graphics.OpenGL4;
using System;

namespace NanoGames.Engine.OpenGLWrappers
{
    /// <summary>
    /// Represents an OpenGL framebuffer, i.e. a texture that can be rendered onto.
    /// </summary>
    internal sealed class Framebuffer : IDisposable
    {
        private readonly int _framebufferId;
        private readonly int _frameTextureId;

        private int _width;
        private int _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Framebuffer"/> class.
        /// </summary>
        public Framebuffer()
        {
            _framebufferId = GL.GenFramebuffer();
            _frameTextureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, _frameTextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 0, 0, 0, 1 });

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _framebufferId);
            GL.FramebufferTexture(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, _frameTextureId, 0);

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        /// <summary>
        /// Gets the framebuffer width.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the framebuffer height.
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Unbind any framebuffer and restore the default render target.
        /// </summary>
        public static void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GL.DeleteTexture(_frameTextureId);
            GL.DeleteFramebuffer(_framebufferId);
        }

        /// <summary>
        /// Sets the framebuffer size.
        /// </summary>
        /// <param name="width">The width, in pixels.</param>
        /// <param name="height">The height, in pixels.</param>
        public void SetSize(int width, int height)
        {
            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;
                GL.BindTexture(TextureTarget.Texture2D, _frameTextureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Binds the framebuffer (result) texture to the current texture unit.
        /// </summary>
        public void BindTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, _frameTextureId);
        }

        /// <summary>
        /// Binds the framebuffer as the current render target.
        /// </summary>
        public void BindFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _framebufferId);
        }
    }
}
