// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NanoGames.Application
{
    /// <summary>
    /// A view that measures and draws the current frames per second.
    /// </summary>
    internal sealed class FpsView
    {
        private readonly Queue<long> _times = new Queue<long>();

        /// <summary>
        /// Updates and renders the view.
        /// </summary>
        /// <param name="terminal">The terminal this view should render to.</param>
        public void Update(Terminal terminal)
        {
            var fontSize = 4;
            var color = new Color(0.60, 0.35, 0.05);

            if (DebugMode.IsEnabled)
            {
                terminal.Graphics.Print(color, fontSize, new Vector(0, 0), "GC " + string.Join("/", Enumerable.Range(0, GC.MaxGeneration + 1).Select(g => GC.CollectionCount(g))));
            }

            var time = Stopwatch.GetTimestamp();

            if (Settings.Instance.ShowFps && _times.Count > 0)
            {
                var fps = (double)Stopwatch.Frequency * _times.Count / (time - _times.Peek());
                var fpsString = ((int)(fps + 0.5)).ToString("D2") + " FPS";

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
