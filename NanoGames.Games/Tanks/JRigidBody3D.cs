// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JRigidBody3D
    {
        public JVertex3D[] Vertices;
        public JLine3D[] Lines;
        public JVertex3D Position;
        public JQuaternion3D Rotation;
        public JVertex3D Scale;
        public Color Color;


        public JRigidBody3D(JVertex3D[] vertices, JLine3D[] lines, JVertex3D pos, JQuaternion3D rot, JVertex3D scale)
        {
            Vertices = vertices;
            Lines = lines;
            Position = pos;
            Rotation = rot;
            Scale = scale;
            Color = new Color(1, 1, 1);
        }

        public JRigidBody3D(JVertex3D[] vertices, JLine3D[] lines, JVertex3D pos, JQuaternion3D rot, JVertex3D scale, Color col)
        {
            Vertices = vertices;
            Lines = lines;
            Position = pos;
            Rotation = rot;
            Scale = scale;
            Color = col;
        }
    }
}
