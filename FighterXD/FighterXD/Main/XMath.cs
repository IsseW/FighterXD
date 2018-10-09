using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    static class XMath
    {
        public static Vector2 RotateVector(this Vector2 v, float angle)
        {
            float sin = (float)Math.Sin(angle * 0.0174532925);
            float cos = (float)Math.Cos(angle * 0.0174532925);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}
