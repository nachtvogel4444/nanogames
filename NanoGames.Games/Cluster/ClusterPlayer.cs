// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Cluster
{
    class ClusterPlayer : Player<ClusterMatch>
    {
        public Vector Position;
        public Vector Velocity;
        public double Angle;
        public double Omega;
        public double Magnification;
        public double MagnificationMin;
        public int Weapon = 0;

        public double View = 100;

        public double Mass = 1;
        public double Size = 3;
        
        public double BoosterPower = 100;

        private int waitFire;

        public List<Segment> Ship = new List<Segment> {
            new Segment( new Vector(-0.1, 1),   new Vector(0.1, 1)),
            new Segment( new Vector(0.1, 1),    new Vector(0.3, 0.5)),
            new Segment( new Vector(0.3, 0.5),  new Vector(0.3, 0.3)),
            new Segment( new Vector(0.3, 0.3),  new Vector(1, -0.1)),
            new Segment( new Vector(1, -0.1),   new Vector(1, -0.5)),
            new Segment( new Vector(1, -0.5),   new Vector(0.3, -0.7)),
            new Segment( new Vector(0.3, -0.7), new Vector(0.3, -1)),
            new Segment( new Vector(0.3, -1),   new Vector(-0.3, -1)),
            new Segment( new Vector(-0.1, 1),   new Vector(-0.3, 0.5)),
            new Segment( new Vector(-0.3, 0.5), new Vector(-0.3, 0.3)),
            new Segment( new Vector(-0.3, 0.3), new Vector(-1, -0.1)),
            new Segment( new Vector(-1, -0.1),  new Vector(-1, -0.5)),
            new Segment( new Vector(-1, -0.5),  new Vector(-0.3, -0.7)),
            new Segment( new Vector(-0.3, -0.7),new Vector(-0.3, -1))
        };

        public List<Segment> Triangle = new List<Segment>
        {
            new Segment(new Vector(0, 2),   new Vector(1, -1)),
            new Segment(new Vector(1, -1),  new Vector(-1, -1)),
            new Segment(new Vector(-1, -1), new Vector(0, 2)),
        };
        public List<Segment> Gun = new List<Segment> {
            new Segment(new Vector(0.7, -0.1), new Vector(0.7, 0.7)),
            new Segment(new Vector(-0.7, -0.1), new Vector(-0.7, 0.7))
        };
        
        public List<LBeam> LBeams = new List<LBeam> { };

        public Vector Heading => Vector.FromAngle(Angle);
        
        public void Birth()
        {
            Velocity = new Vector(0, 0);
            Angle = 0;
            Omega = 0;
            Magnification = MagnificationMin;

            List<Segment> tmp = new List<Segment> { };
            foreach (Segment seg in Ship)
            {
                tmp.Add(seg.Rotated(1.5 * Math.PI));
            }
            Ship = tmp;

            tmp = new List<Segment> { };
            foreach (Segment seg in Gun)
            {
                tmp.Add(seg.Rotated(1.5 * Math.PI));
            }
            Gun = tmp;
        }

        public void Move()
        {
            Vector force = Heading;
            double torque = 0.0;
            double dt = Constants.dt;
            double vis = Constants.Viscosity;

            // check input

            // player moves forward
            if (Input.Left.IsPressed && Input.Right.IsPressed)
            {
                force += BoosterPower * Heading;
            }
            // players moves right
            if (Input.Right.IsPressed && !Input.Left.IsPressed)
            {
                force += 0.5 * BoosterPower * Heading;
                torque += 0.12 * BoosterPower;
            }

            // player moves left
            if (Input.Left.IsPressed && !Input.Right.IsPressed)
            {
                force += 0.5 * BoosterPower * Heading;
                torque -= 0.12 * BoosterPower;
            }

            // viscosity
            force -= 0.05 * Size * vis * Velocity;
            torque -= 0.04 * Size * vis * Size * Omega;

            // calculate acceleration and angular acceleration from forces and torque
            Vector acceleration = force / Mass;
            double angularAcc = torque / Mass;

            // set new position and new velocity
            Position += Velocity * dt + 0.5 * acceleration * dt * dt;
            Velocity += acceleration * dt;

            // set new angle and omega
            Angle += Omega * dt + 0.5 * angularAcc * dt * dt;
            Omega += angularAcc * dt;
        }

        public void MoveLBeams()
        {
            foreach (LBeam lBeam in LBeams)
            {
                lBeam.Move();
            }
        }

        public void Shoot()
        {
            waitFire++;

            if (Input.Fire.WasActivated && waitFire > Constants.LBeam.FireTime)
            {
                Vector p1 = Position + Gun[0].Start.Scaled(Size).Rotated(Angle);
                Vector p2 = Position + Gun[1].Start.Scaled(Size).Rotated(Angle);

                LBeams.Add(new LBeam(p1, Heading));
                LBeams.Add(new LBeam(p2, Heading));

                waitFire = 0;
            }
        }

        public void Collide(ClusterPlayer other)
        {
            if ((other.Position - Position).LengthBBox < other.Size + Size && 
                (other.Position - Position).Length < other.Size + Size)
            {
                Vector dv = Velocity - other.Velocity;
                Vector dx = Position - other.Position;
                double fullMass = Mass + other.Mass;

                Velocity -= 2 * other.Mass / fullMass * Vector.Dot(dv, dx) / dx.SquaredLength * dx;
                other.Velocity += 2 * Mass / fullMass * Vector.Dot(-dv, -dx) / dx.SquaredLength * dx;
            }
        }

        public void Collide(Planet planet)
        {
            if ((planet.Position - Position).LengthBBox < planet.Radius + Size &&
                (planet.Position - Position).Length < planet.Radius + Size)
            {
                Vector dv = Velocity;
                Vector dx = Position - planet.Position;

                if (Vector.Dot(dv, dx) < 0)
                {
                    Velocity -= Constants.bounceFactor * 2 * Vector.Dot(dv, dx) / dx.SquaredLength * dx;
                }
            }
        }

        public void DoMagnification()
        {
            if (Input.Up.WasActivated)
            {
                Magnification *= 2;
                Magnification = Math.Min(Magnification, 16);
                Magnification = Math.Max(Magnification, MagnificationMin);
            }
            
            if (Input.Down.WasActivated)
            {
                Magnification /= 2;
                Magnification = Math.Min(Magnification, 16);
                Magnification = Math.Max(Magnification, MagnificationMin);
            }
        }
        
        public void Draw(ClusterPlayer observer)
        {
            Vector p = Position - observer.Position;
            double m = observer.Magnification;
            IGraphics g = observer.Output.Graphics;

            if (true)
            {
                foreach (Segment part in Ship)
                {
                    Segment seg = part.Scaled(Size).Rotated(Angle).Translated(p).Scaled(m).ToOrigin();
                    g.LLine(LocalColor, seg.Start, seg.End);
                }

                if (Weapon == 0)
                {
                    foreach (Segment part in Gun)
                    {
                        Segment seg = part.Scaled(Size).Rotated(Angle).Translated(p).Scaled(m).ToOrigin();
                        g.LLine(LocalColor, seg.Start, seg.End);
                    }
                }

                foreach (LBeam lbeam in LBeams)
                {
                    lbeam.Draw(observer);
                }
            }
            
            if (Constants.Debug)
            {
                g.CCircle(LocalColor, p.Scaled(m).ToOrigin(), View * m);
                g.LLine(LocalColor, p.Scaled(m).ToOrigin(), (p + 5 * Heading).Scaled(m).ToOrigin());
                g.LLine(Colors.Blue, p.Scaled(m).ToOrigin(), (p + 5 * Velocity.Normalized).Scaled(m).ToOrigin());
            }
        }
    }
}
