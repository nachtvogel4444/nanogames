// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.FallingBlocks
{
    /// <summary>
    /// A match of falling blocks.
    /// </summary>
    internal sealed class FallingBlocksMatch : Match<FallingBlocksPlayer>
    {
        /// <inheritdoc/>
        protected override void Initialize()
        {
            if (Players.Count == 2)
            {
                Players[0].RightPlayer = Players[1];
                Players[1].LeftPlayer = Players[0];
            }
            else if (Players.Count > 2)
            {
                for (int i = 0; i < Players.Count; ++i)
                {
                    int j = (i + 1) % Players.Count;
                    Players[i].RightPlayer = Players[j];
                    Players[j].LeftPlayer = Players[i];
                }
            }

            foreach (var player in Players)
            {
                player.Initialize();
            }
        }

        /// <inheritdoc/>
        protected override void Update()
        {
            foreach (var player in Players)
            {
                player.DrawScreen();
            }
        }
    }
}
