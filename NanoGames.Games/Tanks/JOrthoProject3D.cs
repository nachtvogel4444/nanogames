// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JOrthoProject3D : JMatrix3D
    {
        public JOrthoProject3D(JCamera cam) : base()
        {
            Elements[0, 0] = 1.0 / cam.Width;
            Elements[1, 1] = 1.0 / cam.Heigth;
            Elements[2, 2] = -2.0 / (cam.ZFar - cam.ZNear);
            Elements[2, 3] = -(cam.ZFar + cam.ZNear) / (cam.ZFar - cam.ZNear);
        }
    }
}
