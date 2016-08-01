// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Allows access to the audio interface.
    /// </summary>
    public interface IAudio
    {
        /// <summary>
        /// Plays a certain sound.
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        void Play(Sound sound);
    }
}
