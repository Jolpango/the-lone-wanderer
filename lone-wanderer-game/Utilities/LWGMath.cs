using System;
using Microsoft.Xna.Framework;

namespace LoneWandererGame.Utilities
{
    public static class LWGMath
    {
        public static float closestNumber(float n, float m)
        {
            // find the quotient
            float q = n / m;

            // 1st possible closest number
            float n1 = m * q;

            // 2nd possible closest number
            float n2 = (n * m) > 0 ? (m * (q + 1)) : (m * (q - 1));

            // if true, then n1 is the required closest number
            if (Math.Abs(n - n1) < Math.Abs(n - n2))
                return n1;

            // else n2 is the required closest number
            return n2;
        }

        public static Vector2 closestNumberV2(Vector2 n, Vector2 m)
        {
            return new Vector2(closestNumber(n.X, m.X), closestNumber(n.Y, m.Y));
        }
    }
}
