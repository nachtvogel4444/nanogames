// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JMesh3D
    {
        public JVertex3D[] Vertices;
        public JVertex3D[] VerticesWorld;
        public JLine3D[] Lines;
        public JLine3D[] LinesWorld;
        public JVertex3D Position;
        public JQuaternion3D Rotation;
        public JVertex3D Scale;
        public int NumberOfVertices;


        public JMesh3D(JVertex3D[] vertices, JVertex3D pos, JQuaternion3D rot, JVertex3D scale)
        {
            Vertices = vertices;
            Position = pos;
            Rotation = rot;
            Scale = scale;
            NumberOfVertices = vertices.Length;

            getVerticesWorld();
            getLinesWorld(); // tbc ..
        }


        private void getVerticesWorld()
        {
            JScaleMatrix3D scale = new JScaleMatrix3D(Scale);
            JRotationMatrix3D rotate = new JRotationMatrix3D(Rotation);
            JTranslationMatrix3D translate = new JTranslationMatrix3D(Position);

            JMatrix3D transform = translate * rotate * scale;

            VerticesWorld = new JVertex3D[NumberOfVertices];

            for (int i = 0; i < NumberOfVertices; i++)
            {
                VerticesWorld[i] = transform * Vertices[i];
            }
        }

    }
}
