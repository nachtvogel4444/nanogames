// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// A set of pre-defined sounds.
    /// </summary>
    public static class Sounds
    {
        /// <summary>
        /// A high beep sound.
        /// </summary>
        public static readonly Sound HighBeep = Sound.Chirp(0.1, Pitch.C(2), Pitch.C(3));

        /// <summary>
        /// A low beep sound.
        /// </summary>
        public static readonly Sound LowBeep = Sound.Chirp(0.1, Pitch.C(3), Pitch.C(2));

        /// <summary>
        /// An explosion sound.
        /// </summary>
        public static readonly Sound Explosion = Sound.Sequence(
            Sound.Noise(0.1, new Pitch(0), new Pitch(400)),
            Sound.Noise(0.1, new Pitch(0), new Pitch(350)),
            Sound.Noise(0.1, new Pitch(0), new Pitch(300)),
            Sound.Noise(0.1, new Pitch(0), new Pitch(250)),
            Sound.Noise(0.1, new Pitch(0), new Pitch(200)));

        /// <summary>
        /// A gun fire sound.
        /// </summary>
        public static readonly Sound GunFire = Sound.Sequence(
            Sound.Noise(0.1, new Pitch(800), new Pitch(1000)),
            Sound.Noise(0.1, new Pitch(500), new Pitch(700)),
            Sound.Noise(0.1, new Pitch(200), new Pitch(500)));

        /// <summary>
        /// A grenade fire sound.
        /// </summary>
        public static readonly Sound GrenadeFire = Sound.Sequence(
            Sound.Noise(0.3, new Pitch(400), new Pitch(700)));

        /// <summary>
        /// A walking sound.
        /// </summary>
        public static readonly Sound Walk = Sound.Sequence(
            Sound.Noise(0.1, new Pitch(400), new Pitch(500)));

        /// <summary>
        /// An toc sound.
        /// </summary>
        public static readonly Sound Toc = Sound.Sequence(
            Sound.Noise(0.02, new Pitch(0), new Pitch(400)),
            Sound.Noise(0.02, new Pitch(0), new Pitch(350)),
            Sound.Noise(0.02, new Pitch(0), new Pitch(300)),
            Sound.Noise(0.02, new Pitch(0), new Pitch(250)),
            Sound.Noise(0.02, new Pitch(0), new Pitch(200)));
    }
}
