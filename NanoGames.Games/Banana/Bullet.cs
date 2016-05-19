using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    internal class Bullet
    {
        public double X0;
        public double Y0;
        public double Vx0;
        public double Vy0;
        public bool IsExploded = false;
        public int LifeTime = 0;
        public double X;
        public double Y;
        public double Vx;
        public double Vy;

        public void MoveBullet()
        {
            X = X0 + Vx0 * LifeTime;
            Y = Y0 + Vy0 * LifeTime + 0.5 * Constants.Gravity * LifeTime * LifeTime;
            Vx = Vx0;
            Vy = Vy0 + Constants.Gravity * LifeTime;
            LifeTime++;
        }
    }
}
