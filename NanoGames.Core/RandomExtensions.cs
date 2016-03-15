// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames
{
    /// <summary>
    /// Extension methods for random number generators.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Shuffles a list fairly.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="random">The random source.</param>
        /// <param name="source">The elements to shuffle.</param>
        /// <returns>The shuffled list.</returns>
        public static List<T> Shuffle<T>(this Random random, IEnumerable<T> source)
        {
            var result = new List<T>();
            foreach (var item in source)
            {
                int insertionPoint = random.Next(result.Count + 1);

                if (insertionPoint == result.Count)
                {
                    result.Add(item);
                }
                else
                {
                    var temp = result[insertionPoint];
                    result[insertionPoint] = item;
                    result.Add(temp);
                }
            }

            return result;
        }
    }
}
