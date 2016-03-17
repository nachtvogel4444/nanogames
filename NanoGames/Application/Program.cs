// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System;

namespace NanoGames.Application
{
    /// <summary>
    /// The main program.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var window = new Window())
            {
                window.Run(() => new MainView());
            }
        }
    }
}
