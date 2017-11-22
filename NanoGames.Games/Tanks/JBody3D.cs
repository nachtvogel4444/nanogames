// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JBody3D
    {
        public JRigidBody3D[] RigidBodies { get; set; }
        public JVertex3D Position { get; set; }
        public JQuaternion3D Rotation { get; set; }
        public JVertex3D Scale { get; set; }

    }
}
