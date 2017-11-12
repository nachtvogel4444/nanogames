// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System.Collections.Generic;

namespace NanoGames.Games.Tanks
{
    public class JCube
    {
        public List<JVector3> Vectors = new List<JVector3> { };
        public JVector3 Position;
        public JQuaternion3D Quaternion;

        public JCube(double length)
        {
            Vectors.Add(new JVector3(-length / 2, -length / 2, length / 2));
            Vectors.Add(new JVector3(length / 2, -length / 2, length / 2));
            Vectors.Add(new JVector3(length / 2, length / 2, length / 2));
            Vectors.Add(new JVector3(-length / 2, length / 2, length / 2));
            Vectors.Add(new JVector3(-length / 2, -length / 2, -length / 2));
            Vectors.Add(new JVector3(length / 2, -length / 2, -length / 2));
            Vectors.Add(new JVector3(length / 2, length / 2, -length / 2));
            Vectors.Add(new JVector3(-length / 2, length / 2, -length / 2));

            Position =  new JVector3(0, 0, 0);

            Quaternion = new JQuaternion3D(0, 0, 0, 0);
        }
    }
}
