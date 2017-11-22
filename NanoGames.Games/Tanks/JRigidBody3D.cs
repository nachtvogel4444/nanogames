// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JRigidBody3D
    {
        public JVertex3D[] Vertices { get; set; }
        public JLine3D[] Lines { get; set; }
        public JVertex3D Position { get; set; }
        public JQuaternion3D Rotation { get; set; }
        public JVertex3D Scale { get; set; }
        public Color Color { get; set; }
 

        public JBody3D ToJBody3D()
        {
            JBody3D body = new JBody3D();
            body.Position = Position;
            body.Rotation = Rotation;
            body.Scale = Scale;

            Scale = new JVertex3D(1, 1, 1, 0);
            Position = new JVertex3D(0, 0, 0, 1);
            Rotation = new JQuaternion3D(1, 0, 0, 0);

            body.RigidBodies = new JRigidBody3D[1] { this };

            return body;
        }
    }
}
