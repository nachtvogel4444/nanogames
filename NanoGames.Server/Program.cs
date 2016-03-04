// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Server
{
    /// <summary>
    /// The program class.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var server = new Network.Server())
            {
                Console.WriteLine("NanoGames Server running.");
                Console.ReadKey(false);
            }
        }
    }
}
