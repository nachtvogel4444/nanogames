namespace NanoGames.Games.NanoSoccer
{
    internal class GoalEdge
    {
        private double _startAngle;
        private double _endAngle;

        public GoalEdge(Vector position, double radius, double startAngle, double endAngle)
        {
            Position = position;
            Radius = radius;
            _startAngle = startAngle;
            _endAngle = endAngle;
        }

        public Vector Position
        {
            get; private set;
        }

        public double Radius
        {
            get; private set;
        }

        public void Draw(Graphics g)
        {
            g.CircleSegment(new Color(0, 0.5, 0), Position, Radius, _startAngle, _endAngle);
        }
    }
}
