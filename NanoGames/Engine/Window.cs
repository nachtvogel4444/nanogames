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

        private bool _closing;

        private bool _isFullscreen;

        private string _textInput = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
        {
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
                if (DebugMode.IsEnabled)
                {
                    /* Don't allow fullscreen in debug mode to prevent freezes. */
                    value = false;
                }

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
        /// <typeparam name="TView">The main view type.</typeparam>
        /// <param name="createMainView">The main view factory.</param>
        public void Run<TView>(Func<TView> createMainView)
            where TView : IView, IDisposable
        {
            var previousWindow = _currentWindow.Value;
            _currentWindow.Value = this;

            try
            {
                using (var renderer = new Renderer())
                {
                    var input = new Input();
                    var terminal = new Terminal(renderer);

                    using (var mainView = createMainView())
                    {
                        while (true)
                        {
                            _gameWindow.ProcessEvents();

                            if (_closing)
                            {
                                return;
                            }

                            var width = _gameWindow.Width;
                            var height = _gameWindow.Height;

                            renderer.BeginFrame(width, height);
                            UpdateInput(input);
                            terminal.Input = input;
                            mainView.Update(terminal);
                            renderer.EndFrame();

                            _gameWindow.SwapBuffers();
                        }
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

            var hasFocus = _gameWindow.Focused;

            var keyboardState = Keyboard.GetState();
            input.Back = hasFocus && keyboardState[Key.Escape];
            input.Up = hasFocus && keyboardState[Key.Up];
            input.Down = hasFocus && keyboardState[Key.Down];
            input.Left = hasFocus && keyboardState[Key.Left];
            input.Right = hasFocus && keyboardState[Key.Right];
            input.Fire = hasFocus && (keyboardState[Key.Space] || keyboardState[Key.Enter] || keyboardState[Key.KeypadEnter]);
            input.AltFire = hasFocus && (keyboardState[Key.ControlLeft] || keyboardState[Key.ControlRight]);
            input.Backspace = hasFocus && keyboardState[Key.BackSpace];
            input.Delete = hasFocus && keyboardState[Key.Delete];
        }

        private void OnClose(object sender, EventArgs e)
        {
            _closing = true;
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F10)
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
