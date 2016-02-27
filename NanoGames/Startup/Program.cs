// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using NanoGames.Menu;

namespace NanoGames.Startup
{
    /// <summary>
    /// The main program.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (var mainMenu = new MainMenu())
            {
                using (var window = new Window(mainMenu))
                {
                    window.Run();
                }
            }
        }
    }
}
