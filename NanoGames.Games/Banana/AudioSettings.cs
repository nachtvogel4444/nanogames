// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    internal class AudioSettings
    {
        public bool BulletExploded = false;
        public bool GrenadeExploded = false;
        public bool PlayerWalked = false;
        public bool WeaponSelected = false;
        public bool AngleSet = false;
        public bool LoadingPower = false;
        public bool PlayerShot = false;
        public bool PlayerHitGround = false;
        public bool TimerFiveSecondsToGo = false;
        public bool TimerOneSecondToGo = false;
        public bool NextPlayer = false;

        public void Reset()
        {
            BulletExploded = false;
            GrenadeExploded = false;
            PlayerWalked = false;
            WeaponSelected = false;
            AngleSet = false;
            LoadingPower = false;
            PlayerShot = false;
            PlayerHitGround = false;
            TimerFiveSecondsToGo = false;
            TimerOneSecondToGo = false;
            NextPlayer = false;
        }
    
    }
}
