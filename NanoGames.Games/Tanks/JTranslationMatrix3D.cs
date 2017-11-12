// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JTranslationMatrix3D : JMatrix3D 
    {
        public JTranslationMatrix3D(JVertex3D translation) : base()
        {
            Elements[0, 0] = 1;
            Elements[1, 1] = 1;
            Elements[2, 2] = 1;
            Elements[0, 3] = translation.X;
            Elements[1, 3] = translation.Y;
            Elements[2, 3] = translation.Z;
        }
                     
    }
}
