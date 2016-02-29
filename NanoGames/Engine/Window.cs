// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK;
using OpenTK.Input;
using System;
using System.Threading;

namespace NanoGames.Engine
{
    /// <summary>
    /// The game window.
    /// </summary>
    internal sealed class Window : IDisposable
    {
        private static readonly ThreadLocal<Window> _currentWindow = new ThreadLocal<Window>();

        private readonly GameWindow _gameWindow;
        private readonly IView _mainView;

        private bool _closing;

        private bool _isFullscreen;

        private string _textInput = null;

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
            _gameWindow.Closed += OnClose;
            _gameWindow.KeyDown += OnKeyDown;
            _gameWindow.KeyPress += OnKeyPress;

            _isFullscreen = true;
            IsFullscreen = false;

            _gameWindow.Visible = true;
        }

        /// <summary>
        /// Gets this thread's window.
        /// </summary>
        public static Window Current => _currentWindow.Value;

        /// <summary>
        /// Gets or sets a value indicating whether the window is fullscreen.
        /// </summary>
        public bool IsFullscreen
        {
            get
            {
                return _isFullscreen;
            }

            set
            {
                if (value != _isFullscreen)
                {
                    _isFullscreen = value;
                    if (value)
                    {
                        _gameWindow.WindowState = WindowState.Fullscreen;
                        _gameWindow.WindowBorder = WindowBorder.Hidden;
                        _gameWindow.CursorVisible = false;
                    }
                    else
                    {
                        _gameWindow.WindowState = WindowState.Maximized;
                        _gameWindow.WindowBorder = WindowBorder.Resizable;
                        _gameWindow.CursorVisible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Runs the game loop until the game is quit.
        /// </summary>
        public void Run()
        {
            var previousWindow = _currentWindow.Value;
            _currentWindow.Value = this;

            try
            {
                using (var renderer = new Renderer())
                {
                    var input = new Input();
                    var terminal = new Terminal(renderer);

                    while (true)
                    {
                        _gameWindow.ProcessEvents();

                        if (_closing)
                        {
                            break;
                        }

                        UpdateInput(input);
                        terminal.Input = input;

                        var width = _gameWindow.Width;
                        var height = _gameWindow.Height;

                        renderer.BeginFrame(width, height);
                        _mainView.Update(terminal);
                        renderer.EndFrame();

                        _gameWindow.SwapBuffers();
                    }
                }
            }
            catch (QuitException)
            {
            }
            finally
            {
                _currentWindow.Value = previousWindow;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _gameWindow.Dispose();
        }

        private void UpdateInput(Input input)
        {
            input.Text = _textInput;
            _textInput = null;

            var keyboardState = Keyboard.GetState();
            input.Back = keyboardState[OpenTK.Input.Key.Escape];
            input.Up = keyboardState[OpenTK.Input.Key.Up];
            input.Down = keyboardState[OpenTK.Input.Key.Down];
            input.Left = keyboardState[OpenTK.Input.Key.Left];
            input.Right = keyboardState[OpenTK.Input.Key.Right];
            input.Fire = keyboardState[OpenTK.Input.Key.Space] || keyboardState[OpenTK.Input.Key.Enter] || keyboardState[OpenTK.Input.Key.KeypadEnter];
            input.AltFire = keyboardState[OpenTK.Input.Key.ControlLeft] || keyboardState[OpenTK.Input.Key.ControlRight];
        }

        private void OnClose(object sender, EventArgs e)
        {
            _closing = true;
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.F10)
            {
                _closing = true;
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            _textInput = (_textInput ?? string.Empty) + e.KeyChar;
        }
    }
}
