using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    static class XMath
    {
        public static Texture2D missingTexture;

        public static Vector2 RotateVector(this Vector2 v, float angle)
        {
            float sin = (float)Math.Sin(angle * 0.0174532925);
            float cos = (float)Math.Cos(angle * 0.0174532925);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            return v;
        }


        public static Vector2 ClosestPointOnLine(Vector2 A, Vector2 B, Vector2 P)
        {
            Vector2 AP = P - A;
            Vector2 AB = B - A;

            float magnitudeAB = AB.LengthSquared();
            float ABAPProduct = Vector2.Dot(AP, AB);
            float distance = ABAPProduct / magnitudeAB;

            if (distance < 0) return A;
            else if (distance > 1) return B;
            else return A + AB * distance;
        }

        public static float GetAngle(Vector2 a, Vector2 b)
        {
            float angle = (float)(Math.Atan2(b.Y, b.X) - Math.Atan2(a.Y, a.X));
            return angle;
        }
    }
}
