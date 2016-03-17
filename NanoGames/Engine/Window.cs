// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
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

        private List<KeyEvent> _keyEvents = new List<KeyEvent>();

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
            _gameWindow.Keyboard.KeyRepeat = true;

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
        /// Gets or sets a value indicating whether the rendering is vsynced.
        /// </summary>
        public bool IsVSynced
        {
            get { return _gameWindow.VSync == VSyncMode.On; }
            set { _gameWindow.VSync = value ? VSyncMode.On : VSyncMode.Off; }
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
                            UpdateKeyEvents(terminal.KeyEvents);
                            terminal.Input = GetInput();
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

        private static KeyCode GetKeyCode(Key key)
        {
            switch (key)
            {
                case Key.Up:
                    return KeyCode.Up;

                case Key.Down:
                    return KeyCode.Down;

                case Key.Left:
                    return KeyCode.Left;

                case Key.Right:
                    return KeyCode.Right;

                case Key.Space:
                    return KeyCode.Fire;

                case Key.ControlLeft:
                case Key.ControlRight:
                case Key.AltLeft:
                case Key.AltRight:
                    return KeyCode.AltFire;

                case Key.Escape:
                    return KeyCode.Escape;

                case Key.Enter:
                    return KeyCode.Enter;

                case Key.BackSpace:
                    return KeyCode.Backspace;

                case Key.Delete:
                    return KeyCode.Delete;

                default:
                    return KeyCode.None;
            }
        }

        private void UpdateKeyEvents(List<KeyEvent> keyEvents)
        {
            keyEvents.Clear();
            keyEvents.AddRange(_keyEvents);
            _keyEvents.Clear();
        }

        private Input GetInput()
        {
            var hasFocus = _gameWindow.Focused;
            var keyboardState = hasFocus ? Keyboard.GetState() : default(KeyboardState);

            var input = default(Input);
            input.Up = hasFocus && keyboardState[Key.Up];
            input.Down = hasFocus && keyboardState[Key.Down];
            input.Left = hasFocus && keyboardState[Key.Left];
            input.Right = hasFocus && keyboardState[Key.Right];
            input.Fire = hasFocus && keyboardState[Key.Space];
            input.AltFire = hasFocus && (keyboardState[Key.ControlLeft] || keyboardState[Key.ControlRight] || keyboardState[Key.AltLeft] || keyboardState[Key.AltRight]);

            return input;
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

            var code = GetKeyCode(e.Key);
            if (code != KeyCode.None)
            {
                _keyEvents.Add(new KeyEvent(code));
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            _keyEvents.Add(new KeyEvent(e.KeyChar));
        }
    }
}
