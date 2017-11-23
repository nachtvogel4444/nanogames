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

        private CameraPerspective cam;

        private List<Point> points;
        private List<Segment> segments;
        private List<Triangle> triangles;

        private IGraphics g;

        protected override void Initialize()
        {
            points = new List<Point> { };
            g = Output.Graphics;

            cam = new CameraPerspective(new Vector3(0, 0, 100), new Vector3(0, 0, 1));
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

            // try
            AddStuffToDraw();

            // camera
            cam.CenterToPoint(new Vector3(0, 0, 0));

            // draw
            Draw();

        }

        private void AddStuffToDraw()
        {
            // refresh points, segments and triangles
            points = new List<Point> { };
            segments = new List<Segment> { };
            triangles = new List<Triangle> { };

            // points
            var pwhite = new Point(white, new Vector3(0, 0, 0));
            var pred = new Point(red, new Vector3(30, 0, 10));
            var pgreen = new Point(green, new Vector3(0, 30, 10));
            var pblue = new Point(blue, new Vector3(0, 0, 40));
            points.Add(pwhite);
            points.Add(pred);
            points.Add(pgreen);
            points.Add(pblue);

            // segments
            var lred = new Segment(red, new Vector3(0, 0, 10), new Vector3(20, 0, 10));
            var lgreen = new Segment(green, new Vector3(0, 0, 10), new Vector3(0, 20, 10));
            var lblue = new Segment(blue, new Vector3(0, 0, 10), new Vector3(0, 0, 30));
            segments.Add(lred);
            segments.Add(lgreen);
            segments.Add(lblue);

            // triangles
            var twhite = new Triangle(white, new Vector3(-10, -10, 10), new Vector3(30, 0, 40), new Vector3(0, 30, 40));
            triangles.Add(twhite);

        }

        private void Draw()
        {
            // points
            foreach (Point point in points)
            {
                point.Projection = WorldToProjection(point.Position);
            }

            // segments
            foreach (Segment segment in segments)
            {
                segment.ProjectionStart = WorldToProjection(segment.Start);
                segment.ProjectionStop = WorldToProjection(segment.Stop);

                segment.ZMin = Math.Min(segment.ProjectionStart.Z, segment.ProjectionStop.Z);
                segment.ZMax = Math.Max(segment.ProjectionStart.Z, segment.ProjectionStop.Z);
            }

            // triangles
            foreach (Triangle triangle in triangles)
            {
                triangle.ProjectionA = WorldToProjection(triangle.A);
                triangle.ProjectionB = WorldToProjection(triangle.B);
                triangle.ProjectionC = WorldToProjection(triangle.C);

                triangle.ZMin = Math.Min(triangle.ProjectionA.Z, Math.Min(triangle.ProjectionB.Z, triangle.ProjectionC.Z));
                triangle.ZMax = Math.Max(triangle.ProjectionA.Z, Math.Max(triangle.ProjectionB.Z, triangle.ProjectionC.Z));
            }

            // check if stuff is visible
            foreach (Point point in points)
            {
                point.IsVisible = true;

                // check with every triangle
                foreach (Triangle triangle in triangles)
                {
                    // check if point is in triangle
                    bool trcwz = CWZ(triangle.ProjectionA, triangle.ProjectionB, triangle.ProjectionC);

                    if (trcwz == CWZ(triangle.ProjectionA, triangle.ProjectionB, point.Projection) &&
                        trcwz == CWZ(triangle.ProjectionB, triangle.ProjectionC, point.Projection) &&
                        trcwz == CWZ(triangle.ProjectionC, triangle.ProjectionA, point.Projection))
                    {

                        // point is behind triangle
                        if (point.Projection.Z > triangle.ZMax)
                        {
                            point.IsVisible = false;
                            break;
                        }

                        // point is in range of triagle
                        if (point.Projection.Z >= triangle.ZMin && point.Projection.Z <= triangle.ZMax)
                        {
                            double z = IntersectionZAxis(triangle);

                            if (point.Projection.Z > z)
                            {
                                point.IsVisible = false;
                                break;
                            }
                        }
                    }

                }
            }

            foreach (Segment segment in segments)
            {
                segment.S1 = 0;
                segment.S2 = 1;

                foreach (Triangle triangle in triangles)
                {
                    // calclate Intesection of plane of triangle with segment of segment
                    IntersectionSegmentTriangle intersec = new IntersectionSegmentTriangle(segment, triangle);

                    if (intersec.Exists)
                    {
                        if (intersec.FrontToBack)
                        {
                            segment.S2 = Math.Min(intersec.S, segment.S2);
                        }
                        else
                        {
                            segment.S1 = Math.Max(intersec.S, segment.S1);
                        }
                    }

                    else
                    {
                        // 
                    }
                }
            }

            foreach (Triangle triangle in triangles)
            {
                g.Line(triangle.Color, ProjectionToImage(triangle.ProjectionA), ProjectionToImage(triangle.ProjectionB));
                g.Line(triangle.Color, ProjectionToImage(triangle.ProjectionB), ProjectionToImage(triangle.ProjectionC));
                g.Line(triangle.Color, ProjectionToImage(triangle.ProjectionA), ProjectionToImage(triangle.ProjectionC));
            }

            // draw stuff
            foreach (Point point in points)
            {
                if (point.IsVisible)
                {
                    g.Point(point.Color, ProjectionToImage(point.Projection));
                }
            }

            foreach (Segment segment in segments)
            {
                Vector3 u = segment.ProjectionStop - segment.ProjectionStart;
                Vector3 a = segment.ProjectionStop - segment.S1 * u;
                Vector3 b = segment.ProjectionStop - segment.S2 * u;

                g.Line(segment.Color, ProjectionToImage(a), ProjectionToImage(b));
            }

            // draw info
            g.Print(white, 4, new Vector(10, 10), "POINTS     " + points.Count.ToString());
            g.Print(white, 4, new Vector(10, 16), "SEGMENTS   " + segments.Count.ToString());
            g.Print(white, 4, new Vector(10, 22), "TRIANGLES  " + triangles.Count.ToString());
        }

        private Vector3 WorldToProjection(Vector3 point)
        {
            // world to view space
            Vector3 v1 = (point - cam.Position) * cam.Rotation.Conjugate;

            // view space to projected space
            return new Vector3(v1.X / (v1.Z * Math.Tan(0.5 * cam.FOVX)),
                                     v1.Y / (v1.Z * Math.Tan(0.5 * cam.FOVY)),
                                     -v1.Z);
        }

        private Vector ProjectionToImage(Vector3 v)
        {
            return new Vector(160 * v.X + 160, -100 * v.Y + 100);
        }

        private double IntersectionZAxis(Triangle triangle)
        {
            Vector3 cross = Vector3.Cross(triangle.B, triangle.C);

            return (triangle.A.X * cross.X + triangle.A.Y * cross.Y) / cross.Z + triangle.A.Z;
        }

        private bool CWZ(Vector3 a, Vector3 b, Vector3 c)
        {
            // only for projected space whichis orientated in z direction
            double crossz = (b - a).X * (c - b).Y - (b - a).Y * (c - b).X;

            if (crossz <= 0) { return false; }
            else { return true; }

        }

    }

}
