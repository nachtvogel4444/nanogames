// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK;
using OpenTK.Input;
using System;

namespace NanoGames.Engine
{
    /// <summary>
    /// The game window.
    /// </summary>
    internal sealed class Window : IDisposable
    {
        private readonly GameWindow _gameWindow;
        private readonly IView _mainView;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="mainView">The main view.</param>
        public Window(IView mainView)
        {
            _mainView = mainView;

            _gameWindow = new GameWindow(1280, 720);

            _gameWindow.Title = "NanoGames";
            _gameWindow.VSync = VSyncMode.On;
            _gameWindow.WindowState = WindowState.Maximized;
            _gameWindow.Closed += OnClose;
            _gameWindow.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// Runs the game loop until the game is quit.
        /// </summary>
        public void Run()
        {
            try
            {
                using (var renderer = new Renderer())
                {
                    var terminal = new Terminal(renderer);

                    while (true)
                    {
                        _gameWindow.ProcessEvents();

                        var width = _gameWindow.Width;
                        var height = _gameWindow.Height;

                        renderer.BeginFrame(width, height);
                        _mainView.Refresh(terminal);
                        renderer.EndFrame();

                        _gameWindow.SwapBuffers();
                    }
                }
            }
            catch (QuitException)
            {
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _gameWindow.Dispose();
        }

        private void OnClose(object sender, EventArgs e)
        {
            throw new QuitException();
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F10)
            {
                throw new QuitException();
            }
        }
    }
}
