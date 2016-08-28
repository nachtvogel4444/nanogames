// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Bomberguy
{
    internal class Explosion : AbstractRectbombularThing
    {
        private Type _type;
        private Vector _direction;
        private bool _particles = true;
        private double _beamOpenAngle = 48;
        private Color _centerInnerColor = new Color(1, 1, 1);
        private Color _centerMiddleColor = new Color(0.9, 0.8, 0.5);
        private Color _centerOuterColor = new Color(0.8, 0.4, 0.1);
        private Color _beamMiddleColor = new Color(0.8, 0.4, 0.1);
        private Color _beamOuterColor1 = new Color(1, 0.1, 0.1);
        private Color _beamOuterColor2 = new Color(0.5, 0, 0);

        public Explosion(BomberMatch match, Type type, Vector direction, Vector size) : this(match, type, direction, new Vector(), size)
        {
        }

        public Explosion(BomberMatch match, Type type, Vector direction, Vector position, Vector size) : base(match, true, true, true, position, size)
        {
            this._type = type;
            this._direction = direction;

            match.TimeOnce(1000, () => Destroy());
            match.TimeOnce(100, () => { _particles = false; });
        }

        public enum Type
        {
            CENTER,
            MIDDLE,
            END
        }

        private double DegToRad(double deg) => deg / 360 * 2 * System.Math.PI;

        public override void Draw()
        {
            // determine offset for beam opening by checking four reference directions; not the most elegant solution
            double beamOpeningOffset = 0;  // default case: direction left, opening right
            if (Vector.Dot(_direction, new Vector(0, -1)) > 0) beamOpeningOffset = DegToRad(90); // direction top, opening bottom
            else if (Vector.Dot(_direction, new Vector(1, 0)) > 0) beamOpeningOffset = DegToRad(180); // direction right, opening left
            else if (Vector.Dot(_direction, new Vector(0, 1)) > 0) beamOpeningOffset = DegToRad(270); // direction bottom, opening top

            double beamOpeningStartAngle = beamOpeningOffset + DegToRad(_beamOpenAngle);
            double beamOpeningEndAngle = beamOpeningOffset + DegToRad(360 - _beamOpenAngle);
            double outerCircleOffset = this.Size.X / 3d;
            double circleSize = this.Size.X / 4d;

            switch (_type)
            {
                case Type.CENTER:
                    Match.Output.Graphics.Circle(_centerInnerColor, Center, Match.GetAbsSize(Constants.Bomb.REL_SIZE) * 0.3d / 2d);
                    Match.Output.Graphics.Circle(_centerMiddleColor, Center, Match.GetAbsSize(Constants.Bomb.REL_SIZE) * 0.6d / 2d);
                    Match.Output.Graphics.Circle(_centerOuterColor, Center, Match.GetAbsSize(Constants.Bomb.REL_SIZE) * 0.95 / 2d);

                    if (!_particles) break;
                    Match.Output.Particles.Gravity = new Vector(0, 0);
                    Match.Output.Particles.Velocity = new Vector(0, 0);
                    Match.Output.Particles.Intensity = 0.4;

                    Match.Output.Particles.Circle(_centerInnerColor, Center, Match.GetAbsSize(Constants.Bomb.REL_SIZE) * 0.3d / 2d);
                    Match.Output.Particles.Circle(_centerMiddleColor, Center, Match.GetAbsSize(Constants.Bomb.REL_SIZE) * 0.6d / 2d);
                    Match.Output.Particles.Circle(_centerOuterColor, Center, Match.GetAbsSize(Constants.Bomb.REL_SIZE) * 0.95 / 2d);
                    break;

                case Type.MIDDLE:

                    Match.Output.Graphics.CircleSegment(_beamMiddleColor, this.Center - _direction * outerCircleOffset, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    Match.Output.Graphics.CircleSegment(_beamMiddleColor, this.Center, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    Match.Output.Graphics.CircleSegment(_beamMiddleColor, this.Center + _direction * outerCircleOffset, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);

                    if (!_particles) break;
                    Match.Output.Particles.Gravity = new Vector(0, 0);
                    Match.Output.Particles.Velocity = new Vector(0, 0);
                    Match.Output.Particles.Intensity = 0.4;

                    Match.Output.Particles.CircleSegment(_beamMiddleColor, this.Center - _direction * outerCircleOffset, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    Match.Output.Particles.CircleSegment(_beamMiddleColor, this.Center, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    Match.Output.Particles.CircleSegment(_beamMiddleColor, this.Center + _direction * outerCircleOffset, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    break;
                case Type.END:
                    Match.Output.Graphics.CircleSegment(_beamOuterColor1, this.Center - _direction * outerCircleOffset, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    Match.Output.Graphics.CircleSegment(_beamOuterColor2, this.Center, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);

                    if (!_particles) break;
                    Match.Output.Particles.Gravity = new Vector(0, 0);
                    Match.Output.Particles.Velocity = new Vector(0, 0);
                    Match.Output.Particles.Intensity = 0.1;

                    Match.Output.Particles.CircleSegment(_beamOuterColor1, this.Center - _direction * outerCircleOffset, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    Match.Output.Particles.CircleSegment(_beamOuterColor2, this.Center, circleSize, beamOpeningStartAngle, beamOpeningEndAngle);
                    break;
            }
        }
    }
}
