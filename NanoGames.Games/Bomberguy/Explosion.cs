using System.Timers;

namespace NanoGames.Games.Bomberguy
{
    internal class Explosion : AbstractBomberThing
    {
        private Type type;
        private Timer explosionTimer;

        public Explosion(Type type, BomberMatch match, Vector position, Vector size) : base(match, true, true, position, size)
        {
            this.type = type;
            explosionTimer = new Timer(2000);
            explosionTimer.Elapsed += ExplosionTimer_Elapsed;
            explosionTimer.Start();
        }

        public enum Type
        {
            CENTER,
            ROW,
            COLUMN,
            TOPEND,
            RIGHTEND,
            BOTTOMEND,
            LEFTEND
        }

        public override void Draw(Graphics g)
        {
            g.Circle(new Color(1, 0, 0), Center, Size.X / 10);
        }

        private void ExplosionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            explosionTimer.Stop();
            explosionTimer.Dispose();
            Destroy();
        }
    }
}
