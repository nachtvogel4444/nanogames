// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Tanks
{
    public class JMatrix3D
    {
        public double[,] Elements; // first index -> rows, second index -> colums ("first down, then right")


        public JMatrix3D()
        {
            Elements = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        }


        public static JMatrix3D operator *(JMatrix3D a, JMatrix3D b)
        {
            JMatrix3D product = new JMatrix3D();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    product.Elements[i, j] = a.Elements[i, 0] * b.Elements[0, j] +
                                             a.Elements[i, 1] * b.Elements[1, j] +
                                             a.Elements[i, 2] * b.Elements[2, j] +
                                             a.Elements[i, 3] * b.Elements[3, j];
                }
            }

            return product;
        }
        
        public static JVertex3D operator *(JMatrix3D a, JVertex3D v)
        {
            return new JVertex3D(
                a.Elements[0, 0] * v.X + a.Elements[0, 1] * v.Y + a.Elements[0, 2] * v.Z + a.Elements[0, 3] * v.W,
                a.Elements[1, 0] * v.X + a.Elements[1, 1] * v.Y + a.Elements[1, 2] * v.Z + a.Elements[1, 3] * v.W,
                a.Elements[2, 0] * v.X + a.Elements[2, 1] * v.Y + a.Elements[2, 2] * v.Z + a.Elements[2, 3] * v.W,
                a.Elements[3, 0] * v.X + a.Elements[3, 1] * v.Y + a.Elements[3, 2] * v.Z + a.Elements[3, 3] * v.W);
        }

    }
}
