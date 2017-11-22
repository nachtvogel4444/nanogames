// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Tanks2
{
    public class CameraPerspective
    {
        public Vector3 Position;

        public double FOVX = 65 * Math.PI / 180; 
        public double FOVY = 40 * Math.PI / 180;

        public double ZFar = 300;
        public double ZNear = 10;

        private Vector3 direction;
        private Quaternion rotation;
        private Vector3 zaxis;
        

        public CameraPerspective(Vector3 pos, Vector3 dir)
        {
            zaxis = new Vector3(0, 0, 1);
            Position = pos;
            Direction = dir.Normalized;
        }
        

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                rotation = new Quaternion(zaxis, direction);
            }
        }
        
        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                direction = zaxis * rotation;
            }
        }
        
        public void DrawPoint(IGraphics g, Color c, Vector3 point)
        {
            // projected space <- view space <- world space
            Vector3 viewsp = viewSpace(point);
            Vector3 projsp = projectedSpace(viewsp);

            // image point
            Vector image = imagePoint(projsp);

            // draw
            g.Point(c, image);
        }

        public void DrawLine(IGraphics g, Color c, Vector3 a, Vector3 b)
        {
            Vector aImage = imagePoint(projectedSpace(viewSpace(a)));
            Vector bImage = imagePoint(projectedSpace(viewSpace(b)));

            g.Line(c, aImage, bImage);
        }

        private Vector3 viewSpace(Vector3 point)
        {
            return (point - Position) * Rotation.Conjugate;
        }

        private Vector3 projectedSpace(Vector3 point)
        {
            double x = 1.0 / Math.Tan(0.5 * FOVX) * point.X;
            double y = 1.0 / Math.Tan(0.5 * FOVY) * point.Y;
            double z = -(ZFar / (ZFar - ZNear) + 1);

            return new Vector3(x, y, z);
        }
        
        private Vector imagePoint(Vector3 point)
        {
            double x = 160.0 * point.X + 160.0;
            //double x = -160.0 * point.X + 160.0;
            double y = -100.0 * point.Y + 100.0;

            return new Vector(x, y);
        }

        public void CenterToPoint(Vector3 p)
        {
            Direction = (Position - p).Normalized;
        }
    }
}
