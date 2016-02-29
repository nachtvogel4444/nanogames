// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using NanoGames.Menu;
using System;

namespace NanoGames.Startup
{
    /// <summary>
    /// The main program.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var mainView = new MainView())
            {
                using (var window = new Window(mainView))
                {
                    window.Run();
                }
            }
        }
    }
}
