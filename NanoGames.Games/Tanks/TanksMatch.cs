// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Tanks
{
    internal class TanksMatch : Match<TanksPlayer>
    {
        private Color white = new Color(1, 1, 1);
        private Color red = new Color(1, 0, 0);
        private Color green = new Color(0, 1, 0);
        private Color blue = new Color(0, 0, 1);

        private JBody3D[] bodies;
        private List<JBody3D> bodiesList;

        private JCamera camera;

        private IGraphics graphicsGame;

        private  int frameCounter;

        protected override void Initialize()
        {
            //testMatrices();

            // make all bodies
            bodiesList = new List<JBody3D> { };
            var coordinatesystem = new CoordSystem(new JVertex3D(0, 0, 0, 1), new JVertex3D(20, 20, 20, 0));
            bodiesList.Add(coordinatesystem);

            // bodies to array
            bodies = bodiesList.ToArray();

            // camera
            camera = new JCamera(new JVertex3D(0, 100, 0, 1), new JVertex3D(0, 1, 0, 0));
            //camera.CenterToPoint(new JVertex3D(0, 0, 0, 1));

            // game stuff
            graphicsGame = Output.Graphics;

            frameCounter = 0;
        }

        protected override void Update()
        {
            // check player input
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

            // draw all bodies
            drawBodies(camera); 

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

        private void drawBodies(JCamera cam)
        {
            // get transformation matrix from world space to view space (W2V)
            var translateW2V = new JTranslationMatrix3D(-cam.Position);
            var rotateW2V = new JRotationMatrix3D(-cam.Rotation);
            var tW2V = translateW2V * rotateW2V;

            // get transformation matrix from view space to projected space (V2P)
            var tV2P = new JOrthoProject3D(cam);

            // W2P
            var tW2P = tV2P * tW2V;

            foreach (JBody3D b in bodies)
            {
                // get transformation matrix from body space to world space (B2W)
                var scaleB2W = new JScaleMatrix3D(b.Scale);
                var translateB2W = new JTranslationMatrix3D(b.Position);
                var rotateB2W = new JRotationMatrix3D(b.Rotation);
                
                var tB2W = translateB2W * scaleB2W * rotateB2W;

                foreach (JRigidBody3D rb in b.RigidBodies)
                {
                    // get transformation matrix form rigidbody space to body spacy (RB2B)
                    var scaleRB2B = new JScaleMatrix3D(rb.Scale);
                    var translateRB2B = new JTranslationMatrix3D(rb.Position);
                    var rotateRB2B = new JRotationMatrix3D(rb.Rotation);
                    
                    var tRB2B =  translateRB2B * scaleRB2B * rotateRB2B;

                    // get full transformation matrix to projected space
                    var t = tW2P * tB2W * tRB2B;

                    // draw all vertices
                    foreach (JVertex3D v in rb.Vertices)
                    {
                        var r = t * v;
                        var p = cam.GetImagePoint(r);

                        graphicsGame.Point(rb.Color, p);
                    }

                    // draw all lines
                    foreach (JLine3D l in rb.Lines)
                    {
                        var start = t * l.Start;
                        var stop = t * l.Stop;

                        var p1 = cam.GetImagePoint(start);
                        var p2 = cam.GetImagePoint(stop);

                        graphicsGame.Line(rb.Color, p1, p2);
                    }
                }
            }
        }
    }

}
