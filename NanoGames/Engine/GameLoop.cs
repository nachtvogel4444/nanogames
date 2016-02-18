// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace NanoGames.Engine
{
    /// <summary>
    /// Contains methods for running the game loop.
    /// </summary>
    internal static class GameLoop
    {
        /// <summary>
        /// Runs the game loop until the game is quit.
        /// </summary>
        public static void Run()
        {
            using (var gameWindow = new GameWindow(1280, 720))
            {
                gameWindow.Title = "NanoGames";
                gameWindow.VSync = VSyncMode.On;
                gameWindow.WindowState = WindowState.Maximized;
                gameWindow.Closed += OnClose;
                gameWindow.KeyDown += OnKeyDown;

                try
                {
                    while (true)
                    {
                        var width = gameWindow.Width;
                        var height = gameWindow.Height;
                        GL.Viewport(0, 0, width, height);

                        GL.ClearColor(0, 0, 0, 1);
                        GL.Clear(ClearBufferMask.ColorBufferBit);

                        gameWindow.ProcessEvents();
                        gameWindow.SwapBuffers();
                    }
                }
                catch (QuitException)
                {
                }
            }
        }

        private static void OnClose(object sender, EventArgs e)
        {
            throw new QuitException();
        }

        private static void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F10)
            {
                throw new QuitException();
            }
        }
    }
}
