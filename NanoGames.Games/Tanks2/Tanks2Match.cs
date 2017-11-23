// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Tanks2
{
    internal class Tanks2Match : Match<Tanks2Player>
    {
        private Color white = new Color(1, 1, 1);
        private Color red = new Color(1, 0, 0);
        private Color green = new Color(0, 1, 0);
        private Color blue = new Color(0, 0, 1);

        //private CameraOrtho cam;
        private CameraPerspective cam;
        private Floor floor;


        protected override void Initialize()
        {
            //cam = new CameraOrtho(new Vector3(0, 0, 100), new Vector3(0, 0, 1));
            cam = new CameraPerspective(new Vector3(0, 0, 100), new Vector3(0, 0, 1));
            floor = new Floor(20, 20);
        }

        protected override void Update()
        {
            // check player input
            foreach (var player in Players)
            {
                if (player.Input.Right.WasActivated)
                {
                    cam.Position.X = cam.Position.X + 5;
                }

                if (player.Input.Left.WasActivated)
                {
                    cam.Position.X = cam.Position.X - 5;
                }

                if (player.Input.Up.WasActivated)
                {
                    cam.Position.Y = cam.Position.Y + 5;
                }

                if (player.Input.Down.WasActivated)
                {
                    cam.Position.Y = cam.Position.Y - 5;
                }
            }

            // draw sth
            Output.Graphics.Point(white, new Vector(160, 100));

            // try
            mytest01();
            
        }

        private void mytest01()
        {
            var origin = new Vector3(0, 0, 0);

            // graphics
            var g = Output.Graphics;

            // camera
            cam.CenterToPoint(origin);

            // red point in worldspace
            var redp = new Vector3(20, 0, 10);
            
            // green point in worldspace
            var greenp = new Vector3(0, 20, 10);

            // blue point in worldspace
            var bluep = new Vector3(0, 0, 30);

            // white point in worldspace
            var whitep = new Vector3(0, 0, 10);

            // draw
            cam.DrawPoint(g, red, new Vector3(30, 0, 10));
            cam.DrawPoint(g, green, new Vector3(0, 30, 10));
            cam.DrawPoint(g, blue, new Vector3(0, 0, 40));
            cam.DrawLine(g, red, whitep, redp);
            cam.DrawLine(g, green, whitep, greenp);
            cam.DrawLine(g, blue, whitep, bluep);

            for (int i = 0; i < floor.N; i++)
            {
                cam.DrawLine(g, floor.Colors[i], floor.Start[i], floor.Stop[i]);
            }
        }
        
            
    }

}
