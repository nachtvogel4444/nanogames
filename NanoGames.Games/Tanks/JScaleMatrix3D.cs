// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JScaleMatrix3D : JMatrix3D 
    {
        public JScaleMatrix3D(JVertex3D scale) : base()
        {
            Elements[0, 0] = scale.X;
            Elements[1, 1] = scale.Y;
            Elements[2, 2] = scale.Z;
        }                     
    }
}
