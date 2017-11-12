// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JRotationMatrix3D : JMatrix3D 
    {
        public JRotationMatrix3D(JQuaternion3D q) : base()
        {
            q.Normalize();

            Elements[0, 0] = 1.0 - 2.0 * q.Y * q.Y - 2.0 * q.Z * q.Z;
            Elements[1, 1] = 1.0 - 2.0 * q.X * q.X - 2.0 * q.Z * q.Z;
            Elements[2, 2] = 1.0 - 2.0 * q.X * q.X - 2.0 * q.Y * q.Y;

            Elements[0, 1] = 2.0 * q.X * q.Y - 2.0 * q.Z * q.W;
            Elements[0, 2] = 2.0 * q.X * q.Z + 2.0 * q.Y * q.W;
            Elements[1, 0] = 2.0 * q.X * q.Y + 2.0 * q.Z * q.W;
            Elements[1, 2] = 2.0 * q.Y * q.Z - 2.0 * q.X * q.W;
            Elements[2, 0] = 2.0 * q.X * q.Z - 2.0 * q.Y * q.W;
            Elements[2, 1] = 2.0 * q.Y * q.Z + 2.0 * q.X * q.W;            
        }

    }
}
