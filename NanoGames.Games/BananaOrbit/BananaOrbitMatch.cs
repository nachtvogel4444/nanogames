// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.BananaOrbit
{
    class BananaOrbitMatch : Match<BananaOrbitPlayer>
    {
        protected override void Initialize()
        {

        }

        protected override void Update()
        {
            DrawScreen();
        }

        private void DrawScreen()
        {
            Output.Graphics.Circle(new Color(1, 1, 1), new Vector(160, 100), 20);
        }

        private void PlayAudio()
        {

        }
    }
}
