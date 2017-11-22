// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class CoordSystem : JBody3D
    {
        public JRigidBody3D XAxis;
        public JRigidBody3D YAxis;
        public JRigidBody3D ZAxis;

        public CoordSystem(JVertex3D pos, JVertex3D scale)
        {
            Position = pos;
            Rotation = new JQuaternion3D();
            Scale = scale;
            /*
            XAxis = new JRigidBody3D(new JVertex3D[2] { new JVertex3D(0, 0, 0, 1), new JVertex3D(1, 0, 0, 1) },
                                     new JLine3D[1] { new JLine3D(new JVertex3D(0, 0, 0, 1), new JVertex3D(1, 0, 0, 1)) },
                                     new JVertex3D(0, 0, 0, 1),
                                     new JQuaternion3D(),
                                     new JVertex3D(1, 1, 1, 0),
                                     new Color(1, 0, 0));

            YAxis = new JRigidBody3D(new JVertex3D[2] { new JVertex3D(0, 0, 0, 1), new JVertex3D(0, 1, 0, 1) },
                                     new JLine3D[1] { new JLine3D(new JVertex3D(0, 0, 0, 1), new JVertex3D(0, 1, 0, 1)) },
                                     new JVertex3D(0, 0, 0, 1),
                                     new JQuaternion3D(),
                                     new JVertex3D(1, 1, 1, 0),
                                     new Color(0, 1, 0));

            ZAxis = new JRigidBody3D(new JVertex3D[2] { new JVertex3D(0, 0, 0, 1), new JVertex3D(0, 0, 1, 1) },
                                     new JLine3D[1] { new JLine3D(new JVertex3D(0, 0, 0, 1), new JVertex3D(0, 0, 1, 1)) },
                                     new JVertex3D(0, 0, 0, 1),
                                     new JQuaternion3D(),
                                     new JVertex3D(1, 1, 1, 0),
                                     new Color(0, 0, 1));

            RigidBodies = new JRigidBody3D[3] { XAxis, YAxis, ZAxis };*/
        }
    }
}
