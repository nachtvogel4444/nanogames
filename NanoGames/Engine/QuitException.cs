// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Engine
{
    /// <summary>
    /// Thrown to indicate the game should cleanly shutdown.
    /// </summary>
    [Serializable]
    internal sealed class QuitException : Exception
    {
    }
}
