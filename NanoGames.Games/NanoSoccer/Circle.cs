namespace NanoGames.Games.NanoSoccer
{
    internal interface Circle
    {
        Vector Position { get; set; }

        double Radius { get; }

        Vector Velocity { get; set; }

        double MaximumVelocity { get; }

        double Mass { get; }
    }
}
