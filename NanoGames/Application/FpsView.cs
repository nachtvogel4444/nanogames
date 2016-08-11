﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System.Collections.Generic;
using System.Diagnostics;

namespace NanoGames.Application
{
    /// <summary>
    /// A view that measures and draws the current frames per second.
    /// </summary>
    internal sealed class FpsView : IView
    {
        private readonly Queue<long> _times = new Queue<long>();

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            var fontSize = 6;
            var color = new Color(0.60, 0.35, 0.05);

            if (DebugMode.IsEnabled)
            {
                terminal.Graphics.Print(color, fontSize, new Vector(0, 0), "DEBUG");
                for (int i = 0; i <= System.GC.MaxGeneration; ++i)
                {
                    terminal.Graphics.Print(color, fontSize, new Vector(0, (i + 1) * fontSize), string.Format("GC{0}: {1}", i, System.GC.CollectionCount(i)));
                }
            }

            var time = Stopwatch.GetTimestamp();

            if (Settings.Instance.ShowFps && _times.Count > 0)
            {
                var fps = (double)Stopwatch.Frequency * _times.Count / (time - _times.Peek());
                var fpsString = ((int)(fps + 0.5)).ToString("D2");

                terminal.Graphics.Print(color, fontSize, new Vector(GraphicsConstants.Width - fpsString.Length * fontSize, 0), fpsString);
            }

            if (_times.Count > 128)
            {
                _times.Dequeue();
            }

            _times.Enqueue(time);
        }
    }
}
