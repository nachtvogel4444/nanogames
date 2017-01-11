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

            ConvexPolygon poly01 = new ConvexPolygon();
            ConvexPolygon poly02 = new ConvexPolygon();

            poly01.Add(new Vector(20, 20));
            poly01.Add(new Vector(40, 20));
            poly01.Add(new Vector(40, 40));
            poly01.Add(new Vector(20, 40));
            
            poly02.Add(new Vector(100, 100));
            poly02.Add(new Vector(120, 80));
            poly02.Add(new Vector(130, 110));
            poly02.Add(new Vector(99, 130));

            poly01.DrawAll(Output.Graphics, new Color(1, 1, 1));
            poly02.DrawAll(Output.Graphics, new Color(1, 1, 1));
        }

        private void PlayAudio()
        {

        }
    }
}
