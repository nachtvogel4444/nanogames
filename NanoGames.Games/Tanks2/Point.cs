// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Tanks2
{
    public class Point
    {
        public Vector3 Position;
        public Vector3 Projection;
        public Color Color;
        public bool IsVisible;

        public Point(Color c, Vector3 pos)
        {
            Position = pos;
            Color = c;
        }
    }
}
