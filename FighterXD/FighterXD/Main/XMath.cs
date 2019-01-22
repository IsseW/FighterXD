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

        public const float pi = 3.14159265359f;
        public const float tao = pi * 2;


        public const float deg2Rad = 0.0174532925f;

        public static Vector2 RotateVector(this Vector2 v, float rad)
        {
            float sin = (float)Math.Sin(rad);
            float cos = (float)Math.Cos(rad);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            return v;
        }

        public static Vector2 RotateVectorDegrees(this Vector2 v, float angle)
        {
            angle *= deg2Rad;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            return v;
        }

        public static float Repeat(float r, float f)
        {
            while (r < 0) r += f;
            while (r > f) r -= f;
            return r;
        }

        public static float Clamp(float f, float min, float max)
        {
            if (f < min) f = min;
            if (f > max) f = max;
            return f;
        }

        public static Vector2 ClosestPointOnLine(Vector2 A, Vector2 B, Vector2 P)
        {
            Vector2 AP = P - A;
            Vector2 AB = B - A;

            float magnitudeAB = AB.LengthSquared();
            float ABAPProduct = Vector2.Dot(AP, AB);
            float distance = ABAPProduct / magnitudeAB;

            if (distance <= 0) return A;
            else if (distance >= 1) return B;
            else return A + AB * distance;
        }

        public static float GetAngle(Vector2 a, Vector2 b)
        {
            float angle = (float)(Math.Atan2(b.Y - a.Y, b.X - a.X));
            return angle;
        }

        public static void Debug(params object[] args)
        {
            string s = "";
            if (args.Length > 0) {
                if (args[0] == null) s += "null";
                else s += args[0].ToString();
                for (int i = 1; i < args.Length; i++)
                {
                    
                    if (args[i] == null) s += ", null";
                    else s += ", " + args[i].ToString();
                }
            }
            Console.WriteLine(s);
        }
    }
}
