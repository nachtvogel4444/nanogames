// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK.Graphics.OpenGL4;
using System;
using System.Text;

namespace NanoGames.Engine.OpenGLWrappers
{
    /// <summary>
    /// Represents a shader program.
    /// </summary>
    internal sealed class Shader : IDisposable
    {
        private readonly int _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class
        /// loaded and compiled from an embedded resource.
        /// </summary>
        /// <param name="name">The name/path of the resource to load.</param>
        public Shader(string name)
        {
            int vertexShaderId = LoadProgram(name + ".vs.glsl", ShaderType.VertexShader);
            int fragmentShaderId = LoadProgram(name + ".fs.glsl", ShaderType.FragmentShader);

            _id = GL.CreateProgram();
            GL.AttachShader(_id, vertexShaderId);
            GL.AttachShader(_id, fragmentShaderId);
            GL.LinkProgram(_id);

            int status;
            GL.GetProgram(_id, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                throw new Exception("Error linking shader: " + GL.GetProgramInfoLog(_id));
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GL.DeleteProgram(_id);
        }

        /// <summary>
        /// Binds the shader to the OpenGL context.
        /// </summary>
        public void Bind()
        {
            GL.UseProgram(_id);
        }

        private static int LoadProgram(string path, ShaderType type)
        {
            var source = LoadResource(path);
            var id = GL.CreateShader(type);
            GL.ShaderSource(id, source);

            GL.CompileShader(id);

            int status;
            GL.GetShader(id, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                throw new Exception("Error compiling shader: " + GL.GetShaderInfoLog(id));
            }

            return id;
        }

        private static string LoadResource(string path)
        {
            var stream = typeof(Shader).Assembly.GetManifestResourceStream(path);
            using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
