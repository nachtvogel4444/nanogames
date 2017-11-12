// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;

namespace NanoGames.Games.Tanks
{
    internal class TanksMatch : Match<TanksPlayer>
    {
        private JCube mycube;
        private JCamera mycamera;

        private Color white = new Color(1, 1, 1);
        private Color red = new Color(1, 0, 0);
        private Color green = new Color(0, 1, 0);
        private Color blue = new Color(0, 0, 1);

        private  int frameCounter;

        protected override void Initialize()
        {
            frameCounter = 0;
            
            testMatrices();
            
        }

        protected override void Update()
        {
            foreach (var player in Players)
            {
                if (player.Input.Right.WasActivated)
                {
                }

                if (player.Input.Left.WasActivated)
                {
                }

                if (player.Input.Up.WasActivated)
                {
                }

                if (player.Input.Down.WasActivated)
                {
                }
            }

           // draw something
                    

            frameCounter++;
        }

        private void testMatrices()
        {
            var v = new JVertex3D(3, 4, 5, 1);
            var vr = new JVertex3D(0, 0, 5, 1);

            // trnaslation
            var a = new JTranslationMatrix3D(new JVertex3D(1, 0, 0, 0));
            var b = new JTranslationMatrix3D(new JVertex3D(0, 1, 0, 0));
            var c1 = b * v;
            var c2 = a * b * v;

            // scale
            var s = new JScaleMatrix3D(new JVertex3D(2, 0.5, 1,0));
            var vs = s * v;

            // quaternion
            var qxz = new JQuaternion3D(new JVertex3D(1, 0, 0, 0), new JVertex3D(0, 0, 1, 0));
            var rxz = new JRotationMatrix3D(qxz);
            var vrxz = rxz * vr;

            // rotate
            var qx = new JQuaternion3D(new JVertex3D(1, 0, 0, 0), Math.PI / 2);
            var qy = new JQuaternion3D(new JVertex3D(0, 1, 0, 0), Math.PI / 2);
            var qz = new JQuaternion3D(new JVertex3D(0, 0, 1, 0), Math.PI / 2);
            var rx = new JRotationMatrix3D(qx);
            var ry = new JRotationMatrix3D(qy);
            var rz = new JRotationMatrix3D(qz);
            var vrx = rx * vr;
            var vry = ry * vr;
            var vrz = rz * vr;
        }
    }

}
